using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum CraftingState
    {
        Success,
        Fail,
        Running
    }

    public class CrafterBuildingCmp : BuildingCmp
    {
        private Dictionary<CraftingRecipe, int> craftingRecipesToCraft;

        public CraftingRecipe TargetCraftingRecipe { get; private set; }

        private bool isCrafting;

        public float CurrentCraftingProgress { get; set; } = 0;

        public bool CanCraft
        {
            get
            {
                if (TargetCraftingRecipe == null)
                    return false;

                if (BuildingTemplate.FuelConsumer != null && CurrentFuelCondition <= 0)
                    return false;

                if (IsPowered == false)
                    return false;

                return true;
            }
        }

        public Crafter Crafter { get; private set; }

        public bool IsPrepared { get; set; }

        public CrafterBuildingCmp(BuildingTemplate buildingTemplate, Direction direction)
            : base(buildingTemplate, direction)
        {
            craftingRecipesToCraft = new Dictionary<CraftingRecipe, int>();

            Crafter = buildingTemplate.Crafter;

            if (Crafter.Recipes != null)
            {
                foreach (var recipe in Crafter.Recipes)
                {
                    craftingRecipesToCraft.Add(recipe, 0);
                }
            }
        }

        public override void OnInteractionStarted(InteractionType interactionType, CreatureCmp creature)
        {
            base.OnInteractionStarted(interactionType, creature);

            switch (interactionType)
            {
                case InteractionType.Craft:
                    {
                        isCrafting = true;
                        IsTurnedOn = true;
                    }
                    break;
            }
        }

        public override void OnInteractionEnded(InteractionType interactionType, CreatureCmp creature)
        {
            base.OnInteractionEnded(interactionType, creature);

            switch(interactionType)
            {
                case InteractionType.Craft:
                    {
                        isCrafting = false;
                        IsTurnedOn = false;
                    }
                    break;
            }
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if (CanCraft)
            {
                if (IsInteractionActivated(InteractionType.Craft) == false)
                {
                    ActivateInteraction(InteractionType.Craft);
                    SetInteractionDuration(InteractionType.Craft, TargetCraftingRecipe.Time); // TODO: convert to TimeInHours
                }
            }
            else
            {
                if (IsInteractionActivated(InteractionType.Craft))
                {
                    DeactivateInteraction(InteractionType.Craft);
                }
            }

            if (Crafter.IsAutocrafter && CanCraft && IsPrepared)
            {
                ProcessCraft(null);
            }
            else
            {
                if (isCrafting == false)
                    IsTurnedOn = false;
            }

            if (CanCraft == false)
            {
                IsTurnedOn = false;
            }
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch(interactionType)
            {
                case InteractionType.Craft:
                    {
                        CraftItem();
                    }
                    break;
            }
        }

        public override void ProcessInteraction(InteractionType interactionType, CreatureCmp creature)
        {
            base.ProcessInteraction(interactionType, creature);

            switch(interactionType)
            {
                case InteractionType.Craft:
                    {
                        CurrentCraftingProgress = (int)GetInteractionProgress(interactionType);
                    }
                    break;
            }
        }

        public CraftingState ProcessCraft(CreatureCmp creatureCmp)
        {
            isCrafting = true;
            IsTurnedOn = true;

            CurrentCraftingProgress += Engine.GameDeltaTime;

            if (CurrentCraftingProgress >= TargetCraftingRecipe.Time)
            {
                CraftItem();

                isCrafting = false;
                IsPrepared = false;

                return CraftingState.Success;
            }

            return CraftingState.Running;
        }

        public override void Render()
        {
            base.Render();

            if (isCrafting && TargetCraftingRecipe != null)
            {
                if (TargetCraftingRecipe.ItemToShow != null)
                {
                    TargetCraftingRecipe.ItemToShow.Icon.Draw(new Rectangle((int)Entity.X + (BuildingTemplate.Width * Engine.TILE_SIZE) / 2 - Engine.TILE_SIZE / 2,
                        (int)Entity.Y, Engine.TILE_SIZE, Engine.TILE_SIZE), Color.White);
                }
            }
        }

        public IEnumerable<CraftingRecipe> GetAvailableCraftingRecipes()
        {
            foreach (var kvp in craftingRecipesToCraft)
            {
                CraftingRecipe craftingRecipe = kvp.Key;

                if (GameplayScene.WorldManager.IsCraftingRecipeOpened(craftingRecipe))
                {
                    yield return craftingRecipe;
                }
            }
        }

        public void CraftItem()
        {
            if (TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("knowledge_points"))
            {
                int exp = TargetCraftingRecipe.Result[ItemDatabase.GetItemByName("knowledge_points")];

                GameplayScene.Instance.ProgressTree.AddExp(exp);
            }

            if(TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("flatbread"))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.FIRST_BREAD);
            }

            if (TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("ceramic_pot"))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.FIRST_CERAMIC);
            }

            if (TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("copper"))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.FIRST_COPPER);
            }

            if (TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("bronze"))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.FIRST_BRONZE);
            }

            if (TargetCraftingRecipe.MainItem == ItemDatabase.GetItemByName("iron"))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.FIRST_IRON);
            }

            if(TargetCraftingRecipe.Ingredients.ContainsKey(ItemDatabase.GetItemByName("dry_manure_fuel")))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.ARE_YOU_SURE_IT_DOESNT_STINK);
            }

            CurrentCraftingProgress = 0;

            if (craftingRecipesToCraft[TargetCraftingRecipe] != 100)
                craftingRecipesToCraft[TargetCraftingRecipe] -= 1;

            // Удаляем все ингредиенты рецепта
            foreach (var kvp in TargetCraftingRecipe.Ingredients)
            {
                Item item = kvp.Key;

                int weight = kvp.Value;

                Inventory.PopItem(item, weight);
            }

            if (TargetCraftingRecipe.MainItem.IsVirtual == false)
            {
                // Throwing crafted items
                foreach (var kvp in TargetCraftingRecipe.Result)
                {
                    Item item = kvp.Key;
                    int weight = kvp.Value;

                    GetCenterTile().Inventory.AddCargo(new ItemContainer(item, weight, item.Durability));
                }
            }

            if (craftingRecipesToCraft[TargetCraftingRecipe] > 0)
            {
                CreateSupplyLaborForCraftingRecipeIngredients(TargetCraftingRecipe);
            }

            TargetCraftingRecipe = GetNewTargetCraftingRecipe(TargetCraftingRecipe);
        }

        public void SetCraftingRecipe(CraftingRecipe recipe, int count)
        {
            CreateSupplyLaborForCraftingRecipeIngredients(recipe);

            craftingRecipesToCraft[recipe] = count;

            if (TargetCraftingRecipe == null)
                TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
        }

        public void SetCraftingRecipeV2(CraftingRecipe recipe, int newWeight)
        {
            if (craftingRecipesToCraft.ContainsKey(recipe) == false)
                return;

            int oldWeight = craftingRecipesToCraft[recipe];

            if (oldWeight == newWeight)
                return;

            if (oldWeight > newWeight)
            {
                int difference = oldWeight - newWeight;

                SubCraftingRecipe(recipe, difference);
            }

            if (oldWeight < newWeight)
            {
                if (newWeight == 100)
                {
                    RepeatCraftingRecipe(recipe);
                }
                else
                {
                    int difference = newWeight - oldWeight;

                    AddCraftingRecipe(recipe, difference);
                }
            }
        }

        public void AddCraftingRecipe(CraftingRecipe recipe, int count)
        {
            int oldWeight = craftingRecipesToCraft[recipe];

            if (oldWeight == 0)
            {
                CreateSupplyLaborForCraftingRecipeIngredients(recipe);

                craftingRecipesToCraft[recipe] += count;
            }
            else if (oldWeight == 100)
            {
                CancelOrThrowUnrequiredCraftingRecipeIngredients(recipe);

                craftingRecipesToCraft[recipe] = 0;

                // Если целевым рецептом был текущий, то удаляем текущий рецепт
                if (recipe == TargetCraftingRecipe)
                {
                    TargetCraftingRecipe = null;
                    CurrentCraftingProgress = 0;
                    isCrafting = false;
                    IsPrepared = false;
                }
            }
            else
            {
                craftingRecipesToCraft[recipe] += count;
            }

            if (TargetCraftingRecipe == null)
                TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
        }

        public void SubCraftingRecipe(CraftingRecipe recipe, int count)
        {
            int oldWeight = craftingRecipesToCraft[recipe];

            if (oldWeight == 0)
                return;

            craftingRecipesToCraft[recipe] -= count;
            if (craftingRecipesToCraft[recipe] < 0)
                craftingRecipesToCraft[recipe] = 0;

            if (craftingRecipesToCraft[recipe] == 0)
            {
                if (TargetCraftingRecipe == recipe)
                {
                    TargetCraftingRecipe = null;
                    CurrentCraftingProgress = 0;
                    isCrafting = false;
                    IsPrepared = false;
                }

                CancelOrThrowUnrequiredCraftingRecipeIngredients(recipe);
            }

            if (TargetCraftingRecipe == null)
                TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
        }

        public void RepeatCraftingRecipe(CraftingRecipe recipe)
        {
            int oldWeight = craftingRecipesToCraft[recipe];

            if (oldWeight == 0)
            {
                CreateSupplyLaborForCraftingRecipeIngredients(recipe);
            }
            else if (oldWeight == 100)
            {
                CancelOrThrowUnrequiredCraftingRecipeIngredients(recipe);

                craftingRecipesToCraft[recipe] = 0;

                // Если целевым рецептом был текущий, то удаляем текущий рецепт
                if (recipe == TargetCraftingRecipe)
                {
                    TargetCraftingRecipe = null;
                    CurrentCraftingProgress = 0;
                    isCrafting = false;
                    IsPrepared = false;
                }
            }

            if (oldWeight < 100)
                craftingRecipesToCraft[recipe] = 100;

            if (TargetCraftingRecipe == null)
                TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
        }

        public string GetCraftingRecipeCraftCount(CraftingRecipe recipe)
        {
            int count = craftingRecipesToCraft[recipe];

            if (count == 100)
                return "R";
            else
                return "" + count;
        }

        private void CancelOrThrowUnrequiredCraftingRecipeIngredients(CraftingRecipe craftingRecipe)
        {
            foreach (var kvp in craftingRecipe.Ingredients)
            {
                Item item = kvp.Key;
                int itemCount = kvp.Value;

                int itemRequiredCount = Inventory.GetInventoryRequiredWeight(item);

                if (itemRequiredCount >= itemCount)
                {
                    Inventory.AddRequiredWeight(item, -itemCount);
                }
                else
                {
                    Inventory.AddRequiredWeight(item, -itemRequiredCount);

                    itemCount -= itemRequiredCount;

                    foreach (var itemContainer in Inventory.PopItem(item, itemCount))
                    {
                        GetCenterTile().Inventory.AddCargo(itemContainer);
                    }
                }
            }
        }

        public void UpdateTargetCraftingRecipe()
        {
            if (isCrafting == false)
            {
                TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
            }
        }

        private CraftingRecipe GetNewTargetCraftingRecipe(CraftingRecipe lastCaftingRecipe)
        {
            List<CraftingRecipe> readyToCraftRecipes = new List<CraftingRecipe>();

            // Check if crafter has all required ingredients
            foreach (var kvp in craftingRecipesToCraft)
            {
                CraftingRecipe recipe = kvp.Key;
                int count = kvp.Value;

                // Лимиты крафта работают только тогда, когда крафт поставлен на repeat
                if (count == 100)
                {
                    // Проверяем не превысили ли мы лимит по производству
                    int itemLimit = GameplayScene.Instance.ResourcesLimitManager.GetItemLimit(recipe.MainItem);

                    if (itemLimit != -1)
                    {
                        int totalItemCount = GameplayScene.Instance.TotalResourcesChart.GetItemCount(recipe.MainItem);
                        if (totalItemCount >= itemLimit)
                        {
                            continue;
                        }
                    }
                }

                bool readyToCraft = true;

                if (count > 0 && readyToCraft)
                {
                    bool craft = true;
                    foreach (var ing in recipe.Ingredients)
                    {
                        if (Inventory.GetInventoryFactWeight(ing.Key) < ing.Value)
                        {
                            craft = false;
                            break;
                        }
                    }

                    if (craft)
                    {
                        readyToCraftRecipes.Add(recipe);
                    }
                }
            }

            if (readyToCraftRecipes.Count == 0)
                return null;

            if (lastCaftingRecipe == null)
            {
                return readyToCraftRecipes[0];
            }
            else
            {
                int index = readyToCraftRecipes.IndexOf(lastCaftingRecipe);
                index++;

                if (readyToCraftRecipes.Count == index)
                    index = 0;

                return readyToCraftRecipes[index];
            }
        }

        private void CreateSupplyLaborForCraftingRecipeIngredients(CraftingRecipe craftingRecipe)
        {
            foreach (var kvp in craftingRecipe.Ingredients)
            {
                Item item = kvp.Key;
                int itemCount = kvp.Value;

                Inventory.AddRequiredWeight(item, itemCount);
            }
        }

        protected override void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            base.OnItemAdded(senderInventory, item, weight);

            if (IsBuilt)
            {
                // Проверяем, все ли предметы доставлены для крафта
                if (TargetCraftingRecipe == null)
                {
                    TargetCraftingRecipe = GetNewTargetCraftingRecipe(null);
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            if(Crafter.IsAutocrafter)
            {

            }
            else
            {
                AddAvailableInteraction(InteractionType.Craft, Crafter.LaborType, false);

                MarkInteraction(InteractionType.Craft);
            }

            if (Crafter.IsAutocrafter == false)
            {
                GameplayScene.WorldManager.ManualCrafters.Add(this);
            }
            else
            {
                GameplayScene.WorldManager.AutoCrafterBuildings[Crafter.LaborType].Add(this);
            }

            var craftersSortedByMainItems = GameplayScene.WorldManager.CraftersSortedByMainItems;

            foreach (var recipe in Crafter.Recipes)
            {
                if (craftersSortedByMainItems[recipe.MainItem].Contains(this) == false)
                {
                    craftersSortedByMainItems[recipe.MainItem].Add(this);
                }
            }

            foreach(var recipe in Crafter.Recipes)
            {
                if (GameplayScene.WorldManager.CraftersOpenedItems.Contains(recipe.MainItem) == false)
                {
                    GameplayScene.WorldManager.CraftersOpenedItems.Add(recipe.MainItem);
                }
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            if (Crafter.IsAutocrafter == false)
            {
                GameplayScene.WorldManager.ManualCrafters.Remove(this);
            }
            else
            {
                GameplayScene.WorldManager.AutoCrafterBuildings[Crafter.LaborType].Remove(this);
            }

            var craftersSortedByMainItems = GameplayScene.WorldManager.CraftersSortedByMainItems;

            foreach (var recipe in Crafter.Recipes)
            {
                if (craftersSortedByMainItems[recipe.MainItem].Contains(this))
                {
                    craftersSortedByMainItems[recipe.MainItem].Remove(this);
                }
            }
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                if (TargetCraftingRecipe != null)
                {
                    Item mainItem = TargetCraftingRecipe.MainItem;
                    info += $"{Localization.GetLocalizedText("crafting")}";
                    info += $"\n- {Localization.GetLocalizedText("recipe")}: {mainItem.Name} x{TargetCraftingRecipe.Result[mainItem]}";
                    info += $"\n- {Localization.GetLocalizedText("progress")}: {(int)CurrentCraftingProgress}/{TargetCraftingRecipe.Time}";
                }
            }

            return info;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.CurrentCraftingProgress = CurrentCraftingProgress;
                saveData.CraftingRecipesToCraft = new Dictionary<int, int>();

                foreach (var kvp in craftingRecipesToCraft)
                {
                    saveData.CraftingRecipesToCraft.Add(kvp.Key.Id, kvp.Value);
                }

                saveData.IsCrafterPrepared = IsPrepared;
            }

            return saveData;
        }
    }
}