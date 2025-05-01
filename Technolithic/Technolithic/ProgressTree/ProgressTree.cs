using System;
using System.Collections.Generic;

namespace Technolithic
{

    public enum TechnologyState
    {
        Unlocked,
        ReadyToUnlock,
        Locked
    }

    public class ProgressTree
    {

        public int CurrentExp { get; private set; }

        public Dictionary<Item, bool> UnlockedItems = new Dictionary<Item, bool>();
        public Dictionary<MyAction, bool> UnlockedActions = new Dictionary<MyAction, bool>();

        public Dictionary<Technology, TechnologyState> TechnologiesStates = new Dictionary<Technology, TechnologyState>();

        public Action<int> OnExpAddedCallback { get; set; }
        public Action<Technology> TechnologyUnlocked { get; set; }
        public Action<BuildingTemplate> BuildingUnlocked { get; set; }
        public Action<BuildingTemplate> BuildingJustUnlocked { get; set; }
        public Action<BuildingTemplate> JustUnlockedBuildingRemoved { get; set; }

        private Dictionary<BuildingTemplate, bool> unlockedBuildings = new Dictionary<BuildingTemplate, bool>();
        private HashSet<AnimalTemplate> unlockedAnimals = new HashSet<AnimalTemplate>();

        private Dictionary<Technology, List<Technology>> parentAndChildren = new Dictionary<Technology, List<Technology>>();

        private HashSet<BuildingTemplate> justUnlockedBuildings = new HashSet<BuildingTemplate>();

        public ProgressTree(ProgressTreeSaveData progressTreeSaveData)
        {
            foreach (var kvp in TechnologyDatabase.Technologies)
            {
                AddTechnology(kvp.Value);
            }

            // Инициализация коллекций
            foreach (var kvp in Engine.Instance.Buildings)
            {
                var building = kvp.Value;

                unlockedBuildings.Add(building, false);
            }

            foreach (var kvp in ItemDatabase.Items)
            {
                var item = kvp.Value;

                UnlockedItems.Add(item, false);
            }

            foreach(Item item in Engine.Instance.DefaultItemsUnlocks)
            {
                UnlockedItems[item] = true;
            }

            foreach(BuildingTemplate buildingTemplate in Engine.Instance.DefaultBuildingsUnlocks)
            {
                unlockedBuildings[buildingTemplate] = true;
            }

            // создание ключей родитель - дети
            foreach (var kvp in TechnologiesStates)
            {
                Technology child = kvp.Key;
                List<Technology> parentTechnologies = child.ParentTechnologies;

                if (parentTechnologies == null || parentTechnologies.Count == 0)
                    continue;

                foreach (var parent in parentTechnologies)
                {
                    parentAndChildren[parent].Add(child);
                }
            }

            if(progressTreeSaveData != null)
            {
                CurrentExp = progressTreeSaveData.CurrentExp;

                List<Technology> uncheckedTechnologies = new List<Technology>();
                foreach(var kvp in TechnologyDatabase.Technologies)
                {
                    uncheckedTechnologies.Add(kvp.Value);
                }

                if (progressTreeSaveData.JustUnlockedBuildingTemplates != null)
                {
                    foreach (var buildingTemplateJson in progressTreeSaveData.JustUnlockedBuildingTemplates)
                    {
                        if (Engine.Instance.Buildings.TryGetValue(buildingTemplateJson, out BuildingTemplate buildingTemplate))
                        {
                            justUnlockedBuildings.Add(buildingTemplate);
                        }
                    }
                }

                Dictionary<Technology, TechnologyState> checkedTechnologies = new Dictionary<Technology, TechnologyState>();

                for (int i = uncheckedTechnologies.Count - 1; i >= 0; i--)
                {
                    Technology technologyToCheck = uncheckedTechnologies[i];
                    
                    if (technologyToCheck.ParentTechnologies == null)
                    {
                        if (progressTreeSaveData.TechnologiesStates.ContainsKey(technologyToCheck.Id) == false)
                        {
                            checkedTechnologies.Add(technologyToCheck, TechnologyState.ReadyToUnlock);
                        }
                        else
                        {
                            TechnologyState technologyState = (TechnologyState)progressTreeSaveData.TechnologiesStates[technologyToCheck.Id];

                            if(technologyState == TechnologyState.Locked)
                            {
                                checkedTechnologies.Add(technologyToCheck, TechnologyState.ReadyToUnlock);
                            }
                            else
                            {
                                checkedTechnologies.Add(technologyToCheck, technologyState);
                            }
                            
                        }

                        uncheckedTechnologies.Remove(technologyToCheck);
                    }
                }

                while(uncheckedTechnologies.Count > 0)
                {
                    for(int i = uncheckedTechnologies.Count - 1; i >= 0; i--)
                    {
                        Technology technologyToCheck = uncheckedTechnologies[i];
                        bool allParentsInChecked = true;

                        foreach(var parentTechnology in technologyToCheck.ParentTechnologies)
                        {
                            if(checkedTechnologies.ContainsKey(parentTechnology) == false)
                            {
                                allParentsInChecked = false;
                                break;
                            }
                        }

                        if(allParentsInChecked)
                        {
                            bool allParentsUnlocked = true;

                            foreach(var parentTechnology in technologyToCheck.ParentTechnologies)
                            {
                                if(checkedTechnologies[parentTechnology] == TechnologyState.ReadyToUnlock ||
                                    checkedTechnologies[parentTechnology] == TechnologyState.Locked)
                                {
                                    allParentsUnlocked = false;
                                    break;
                                }
                            }

                            if(allParentsUnlocked == false)
                            {
                                checkedTechnologies.Add(technologyToCheck, TechnologyState.Locked);
                                uncheckedTechnologies.Remove(technologyToCheck);
                            }
                            else
                            {
                                if(progressTreeSaveData.TechnologiesStates.ContainsKey(technologyToCheck.Id))
                                {
                                    TechnologyState savedTechnologyState = (TechnologyState)progressTreeSaveData.TechnologiesStates[technologyToCheck.Id];
                                    if(savedTechnologyState == TechnologyState.Locked ||
                                        savedTechnologyState == TechnologyState.ReadyToUnlock)
                                    {
                                        checkedTechnologies.Add(technologyToCheck, TechnologyState.ReadyToUnlock);
                                        uncheckedTechnologies.Remove(technologyToCheck);
                                    }
                                    else if(savedTechnologyState == TechnologyState.Unlocked)
                                    {
                                        checkedTechnologies.Add(technologyToCheck, TechnologyState.Unlocked);
                                        uncheckedTechnologies.Remove(technologyToCheck);
                                    }
                                }
                                else
                                {
                                    checkedTechnologies.Add(technologyToCheck, TechnologyState.ReadyToUnlock);
                                    uncheckedTechnologies.Remove(technologyToCheck);
                                }
                            }
                        }
                    }
                }

                foreach(var kvp in checkedTechnologies)
                {
                    Technology technology = kvp.Key;
                    TechnologyState technologyState = kvp.Value;

                    TechnologiesStates[technology] = technologyState;

                    if (technologyState == TechnologyState.Unlocked)
                    {
                        // Разблокируем предметы
                        if (technology.UnlockedItems != null)
                            foreach (var item in technology?.UnlockedItems)
                                UnlockedItems[item] = true;

                        // Разблокируем строения
                        if (technology.UnlockedBuildings != null)
                            foreach (var building in technology?.UnlockedBuildings)
                                unlockedBuildings[building] = true;

                        // Разблокируем животных
                        if (technology.UnlockedAnimals != null)
                            foreach (var animal in technology?.UnlockedAnimals)
                                unlockedAnimals.Add(animal);

                        if (technology.UnlockedActions != null)
                            foreach (var techAction in technology?.UnlockedActions)
                                UnlockedActions[techAction] = true;
                    }
                }
            }
        }

        public void AddTechnology(Technology technology)
        {
            if(technology.ParentTechnologies == null)
                TechnologiesStates.Add(technology, TechnologyState.ReadyToUnlock);
            else
                TechnologiesStates.Add(technology, TechnologyState.Locked);

            parentAndChildren.Add(technology, new List<Technology>());
        }

        private int lastTechnologiesCount = 0;

        public void AddExp(int value)
        {
            CurrentExp += value;

            int newTechnologiesCount = 0;
            foreach(var kvp in TechnologiesStates)
            {
                Technology technology = kvp.Key;
                TechnologyState technologyState = kvp.Value;

                if (technologyState == TechnologyState.ReadyToUnlock && technology.RequiredXP <= CurrentExp)
                    newTechnologiesCount++;
            }

            if(lastTechnologiesCount < newTechnologiesCount)
            {
                lastTechnologiesCount = newTechnologiesCount;
                GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>().AddNotification(Localization.GetLocalizedText("new_technologies_available"));
            }

            OnExpAddedCallback?.Invoke(CurrentExp);
        }

        public void UnlockTechnology(Technology technology)
        {
            // Разблокировка технологии
            TechnologiesStates[technology] = TechnologyState.Unlocked;

            // Разблокируем предметы
            if (technology.UnlockedItems != null)
                foreach (var item in technology?.UnlockedItems)
                    UnlockedItems[item] = true;

            // Разблокируем строения
            if (technology.UnlockedBuildings != null)
            {
                foreach (var building in technology?.UnlockedBuildings)
                {
                    unlockedBuildings[building] = true;

                    if(justUnlockedBuildings.Contains(building) == false)
                    {
                        justUnlockedBuildings.Add(building);

                        BuildingJustUnlocked?.Invoke(building);
                    }
                }
            }

            // Разблокируем животных
            if(technology.UnlockedAnimals != null)
            {
                foreach(var animal in technology.UnlockedAnimals)
                {
                    unlockedAnimals.Add(animal);
                }
            }

            // Возможность разблокировать дочерние технологии (только если все родительские технологии были разблокированы)
            foreach (var child in parentAndChildren[technology])
            {
                bool readyToUnlock = true;

                foreach(var parent in child.ParentTechnologies)
                {
                    if(TechnologiesStates[parent] != TechnologyState.Unlocked)
                    {
                        readyToUnlock = false;
                        break;
                    }
                }

                if(readyToUnlock)
                    TechnologiesStates[child] = TechnologyState.ReadyToUnlock;
            }

            CurrentExp -= technology.RequiredXP;

            lastTechnologiesCount = 0;

            TechnologyUnlocked?.Invoke(technology);
        }

        public bool IsTechnologyUnlocked(Technology technology)
        {
            return TechnologiesStates[technology] == TechnologyState.Unlocked;
        }

        public bool IsJustUnlockedBuilding(BuildingTemplate buildingTemplate)
        {
            return justUnlockedBuildings.Contains(buildingTemplate);
        }

        public void RemoveJustUnlockedBuilding(BuildingTemplate buildingTemplate)
        {
            justUnlockedBuildings.Remove(buildingTemplate);

            JustUnlockedBuildingRemoved?.Invoke(buildingTemplate);
        }

        public bool AnyJustUnlockedBuildingsInBuildingCategory(BuildingCategory buildingCategory)
        {
            foreach (var buildingTemplate in justUnlockedBuildings)
            {
                if (buildingTemplate.BuildingCategory == buildingCategory)
                {
                    return true;
                }
            }

            return false;
        }

        public ProgressTreeSaveData GetSaveData()
        {
            ProgressTreeSaveData progressTreeSaveData = new ProgressTreeSaveData();

            progressTreeSaveData.CurrentExp = CurrentExp;
            progressTreeSaveData.TechnologiesStates = new Dictionary<int, int>();
            foreach (var kvp in TechnologiesStates)
            {
                progressTreeSaveData.TechnologiesStates.Add(kvp.Key.Id, (int)kvp.Value);
            }

            progressTreeSaveData.JustUnlockedBuildingTemplates = new List<string>();
            foreach (var buildingTemplate in justUnlockedBuildings)
            {
                progressTreeSaveData.JustUnlockedBuildingTemplates.Add(buildingTemplate.Json);
            }

            return progressTreeSaveData;
        }

        public IEnumerable<BuildingTemplate> GetUnlockedBuildingTemplatesEnumerable()
        {
            if(unlockedBuildings != null)
            {
                foreach(var kvp in unlockedBuildings)
                {
                    if(kvp.Value)
                    {
                        yield return kvp.Key;
                    }
                }
            }
        }

        public IEnumerable<AnimalTemplate> GetUnlockedAnimalTemplates()
        {
            foreach (var animal in unlockedAnimals)
            {
                yield return animal;
            }
        }

        public bool IsBuildingUnlocked(BuildingTemplate buildingTemplate)
        {
            if (unlockedBuildings == null)
                return false;

            if (unlockedBuildings.ContainsKey(buildingTemplate) == false)
                return false;

            return unlockedBuildings[buildingTemplate];
        }

        public bool IsAnimalUnlocked(AnimalTemplate animalTemplate)
        {
            return unlockedAnimals.Contains(animalTemplate);
        }

        public bool AreAllTechnologiesUnlocked()
        {
            foreach (var kvp in TechnologiesStates)
            {
                if (kvp.Value != TechnologyState.Unlocked)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
