using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalPenBuildingCmp : BuildingCmp
    {

        public int FreeSlots { get => BuildingTemplate.AnimalPenData.Slots - animals.Count; }

        private HashSet<AnimalCmp> animals = new HashSet<AnimalCmp>();

        private Dictionary<AnimalTemplate, bool> animalsFilters = new Dictionary<AnimalTemplate, bool>();

        private Item hayItem;

        private MyTexture manureTexture;
        private Item manureItem;

        private const int MAX_MANURE_PROGRESS = 100;
        public float CurrentManureProgress { get; set; } = 0;

        private const float MAX_PERCENT_PER_DAY = 75;

        public AnimalPenBuildingCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {

        }

        public override void Begin()
        {
            base.Begin();

            manureTexture = ResourceManager.GetTexture("manure");
            manureItem = ItemDatabase.GetItemByName("manure");
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = Inventory.GetInventoryFactWeight(hayItem) > 0;

            if(IsFullOfManure())
            {
                if(IsInteractionActivated(InteractionType.AutoCleanPen) == false)
                {
                    ActivateInteraction(InteractionType.AutoCleanPen);
                }
            }
            else
            {
                if (IsInteractionActivated(InteractionType.AutoCleanPen))
                {
                    DeactivateInteraction(InteractionType.AutoCleanPen);
                }
            }

            if (IsFullOfManure() == false)
            {
                float progressPerDay = GetPercentPerDayValueBasedOnInsideAnimalsAmount();
                float progressPerMinute = ConvertProgressPerDayToMinute(progressPerDay);
                CurrentManureProgress += progressPerMinute * Engine.GameDeltaTime;
            }
        }

        public override void Render()
        {
            base.Render();

            if(IsFullOfManure())
            {
                manureTexture.Draw(Entity.Position);
            }
        }

        public bool IsFullOfManure()
        {
            return CurrentManureProgress >= MAX_MANURE_PROGRESS;
        }

        private float ConvertProgressPerDayToMinute(float progressPerDay)
        {
            return progressPerDay / (WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
        }

        private float GetPercentPerDayValueBasedOnInsideAnimalsAmount()
        {
            float animalsAmountInside = GetAnimalsAmountInside();

            float penFullnessPercent = animalsAmountInside / (float)BuildingTemplate.AnimalPenData.Slots;

            return MAX_PERCENT_PER_DAY * penFullnessPercent;
        }

        private int GetAnimalsAmountInside()
        {
            int amount = 0;

            foreach(var animal in animals)
            {
                if(animal.Movement.CurrentTile.Entity == this.Entity)
                {
                    amount++;
                }
            }

            return amount;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            AddAvailableInteraction(InteractionType.AutoCleanPen, LaborType.Ranching, false);
            SetInteractionDuration(InteractionType.AutoCleanPen, 1 * WorldState.MINUTES_PER_HOUR);

            MarkInteraction(InteractionType.AutoCleanPen);

            foreach (var animalTemplate in BuildingTemplate.AnimalPenData.GetAllowedAnimalTemplates())
            {
                animalsFilters.Add(animalTemplate, true);
            }

            GameplayScene.WorldManager.AnimalPenBuildings.Add(this);

            hayItem = ItemDatabase.GetItemByName("hay");

            int hayFactWeight = Inventory.GetInventoryFactWeight(hayItem);
            int hayToDeliver = BuildingTemplate.AnimalPenData.Slots - hayFactWeight;
            if (hayToDeliver > 0)
            {
                Inventory.AddRequiredWeight(hayItem, hayToDeliver);
            }
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.AutoCleanPen:
                    {
                        CleanManure();
                    }
                    break;
            }
        }

        public void CleanManure()
        {
            CurrentManureProgress = 0;

            GetCenterTile().Inventory.AddCargo(manureItem, 5);

            DeactivateInteraction(InteractionType.AutoCleanPen);
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.WorldManager.AnimalPenBuildings.Remove(this);

            foreach(var animal in animals)
            {
                animal.TargetAnimalPen = null;
            }

            animals.Clear();
        }

        public void AddAnimal(AnimalCmp animalCmp)
        {
            animals.Add(animalCmp);
            animalCmp.TargetAnimalPen = this;
        }

        public void RemoveAnimal(AnimalCmp animalCmp)
        {
            animals.Remove(animalCmp);
            animalCmp.TargetAnimalPen = null;
        }

        public IEnumerable<KeyValuePair<AnimalTemplate, bool>> GetAnimalsFilters()
        {
            foreach(var kvp in animalsFilters)
            {
                yield return kvp;
            }
        }

        public bool GetAnimalTemplateFilter(AnimalTemplate animalTemplate)
        {
            if (animalsFilters.ContainsKey(animalTemplate) == false)
                return false;

            return animalsFilters[animalTemplate];
        }

        public void SetAnimalFilter(AnimalTemplate animalTemplate, bool flag)
        {
            if (animalsFilters.ContainsKey(animalTemplate) == false)
                return;

            if(flag == false)
            {
                foreach (var animal in animals)
                {
                    if(animal.AnimalTemplate == animalTemplate)
                    {
                        animal.TargetAnimalPen = null;
                    }
                }

                animals.RemoveWhere(x => x.AnimalTemplate == animalTemplate);
            }

            animalsFilters[animalTemplate] = flag;
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                float percentPerDay = GetPercentPerDayValueBasedOnInsideAnimalsAmount();

                info += $"{manureItem.Name}:\n" +
                    $"- {Localization.GetLocalizedText("progress")}: {(int)CurrentManureProgress}% (+{percentPerDay}% {Localization.GetLocalizedText("per_day")})\n";
            }

            return info;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            saveData.AnimalsFilters = new Dictionary<string, bool>();

            saveData.CurrentManureProgress = CurrentManureProgress;

            foreach (var kvp in animalsFilters)
            {
                saveData.AnimalsFilters.Add(kvp.Key.Json, kvp.Value);
            }

            return saveData;
        }

    }
}
