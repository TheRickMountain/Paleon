using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Technolithic
{
    public class CommandLineUIScript : MScript
    {

        private MTextInputScript textInput;

        private CommandExecutor commandExecutor;

        private List<string> lastEnteredCommands;

        private int lastEnteredCommandNumber = 0;

        public CommandLineUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            commandExecutor = new CommandExecutor(Assembly.GetExecutingAssembly());

            lastEnteredCommands = new List<string>();

            textInput = ParentNode.GetChildByName("TextInput").GetComponent<MTextInputScript>();
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            if (MInput.Keyboard.Pressed(Keys.Enter))
            {
                string enteredCommand = textInput.CurrentText;

                if (string.IsNullOrEmpty(enteredCommand) == false)
                {
                    if ((lastEnteredCommands.Count > 0 && lastEnteredCommands[lastEnteredCommands.Count - 1] == enteredCommand) == false)
                    {
                        lastEnteredCommands.Add(enteredCommand);
                    }

                    lastEnteredCommandNumber = lastEnteredCommands.Count;

                    ExecuteCommand(enteredCommand);

                    textInput.ResetText();
                }
            }

            if (MInput.Keyboard.Pressed(Keys.Up))
            {
                if (lastEnteredCommandNumber > 0)
                {
                    lastEnteredCommandNumber--;

                    textInput.CurrentText = lastEnteredCommands[lastEnteredCommandNumber];
                }
            }

            if (MInput.Keyboard.Pressed(Keys.Down))
            {
                if (lastEnteredCommandNumber < lastEnteredCommands.Count)
                {
                    lastEnteredCommandNumber++;
                    if (lastEnteredCommandNumber == lastEnteredCommands.Count)
                    {
                        lastEnteredCommandNumber = lastEnteredCommands.Count - 1;
                    }

                    textInput.CurrentText = lastEnteredCommands[lastEnteredCommandNumber];
                }
            }
        }

        private void ExecuteCommand(string input)
        {
            try
            {
                if (commandExecutor.TryExecute(input, out string error))
                {
                    return;
                }

                ShowError(error);
            }
            catch (TargetInvocationException exception)
            {
                Exception commandException = exception.InnerException ?? exception;

                Program.WriteWarningLog(commandException.ToString());

                ShowError($"Command failed: {commandException.Message}");
            }
        }

        private static void ShowError(string message)
        {
            GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                .AddNotification(message, NotificationLevel.WARNING);
        }

        [Command("edge_scrolling")]
        public static void SetEdgeScrolling(bool value)
        {
            GameSettings.EdgeScrollingCamera = value;
        }

        [Command("unlock_all_technologies")]
        public static void UnlockAllTechnologies()
        {
            foreach (var kvp in TechnologyDatabase.Technologies)
            {
                Technology technology = kvp.Value;
                GameplayScene.Instance.ProgressTree.UnlockTechnology(technology);
            }
        }

        [Command("add_growing_progress")]
        public static void AddGrowingProgressToPlant(int value)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            FarmPlot farmPlot = entity?.Get<FarmPlot>();
            if (farmPlot == null)
                return;

            farmPlot.AddGrowingProgress(value);
        }

        [Command("equip")]
        private static void Equip(string itemName)
        {
            Item item = ItemDatabase.GetItemByName(itemName);
            if (item == null)
                return;

            foreach (var settler in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Has<SettlerCmp>())
                .Select(x => x.Get<SettlerCmp>()))
            {
                if (settler.IsSelected)
                {
                    if (item.Tool != null)
                    {
                        settler.CreatureEquipment.EquipTool(new ItemContainer(item, 1, item.Durability));
                    }

                    if (item.Outfit != null)
                    {
                        if (item.Outfit.IsTop)
                        {
                            settler.CreatureEquipment.TopClothingItemContainer = new ItemContainer(item, 1, item.Durability);
                        }
                        else
                        {
                            settler.CreatureEquipment.ClothingItemContainer = new ItemContainer(item, 1, item.Durability);
                        }
                    }
                }
            }
        }

        [Command("kill")]
        private static void Kill()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            creature?.Die(null);
        }

        [Command("rename")]
        private static void Rename(string name)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                creature.Name = name;
            }
        }

        [Command("set")]
        private static void SetAttribute(AttributeType attributeType, int value)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                var attribute = creature.CreatureStats.GetAttribute(attributeType);
                if (attribute != null)
                {
                    attribute.CurrentValue = value;
                }
            }
        }

        [Command("add_status_effect")]
        private static void AddStatusEffect(StatusEffectId statusEffectId)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                creature.StatusEffectsManager.AddStatusEffect(statusEffectId);
            }
        }

        [Command("remove_status_effect")]
        private static void RemoveStatusEffect(StatusEffectId statusEffectId)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                creature.StatusEffectsManager.RemoveStatusEffect(statusEffectId);
            }
        }

        [Command("clean_manure")]
        private static void CleanManure()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalPenBuildingCmp animalPen = entity?.Get<AnimalPenBuildingCmp>();
            if (animalPen != null && animalPen.IsBuilt)
            {
                animalPen.CleanManure();
            }
        }

        [Command("ready_to_fertilize")]
        private static void SetReadyToFertilize()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalCmp animal = entity?.Get<AnimalCmp>();
            if (animal != null)
            {
                animal.IsReadyToFertilization = true;
            }
        }

        [Command("exit_game")]
        private static void ExitGame()
        {
            Engine.Instance.Exit();
        }

        [Command("add_exp")]
        private static void AddExp(int value)
        {
            GameplayScene.Instance.ProgressTree.AddExp(value);
        }

        [Command("instant_build")]
        private static void SetInstantBuild(bool value)
        {
            GameplayScene.BuildingsRequiredBuilding = !value;
        }

        [Command("instant_destruct")]
        private static void SetInstantDestruct(bool value)
        {
            GameplayScene.BuildingsRequiredDestructing = !value;
        }

        [Command("show_water_chunks")]
        private static void SetShowWaterChunks(bool value)
        {
            GameplayScene.ShowWaterChunks = value;
        }

        [Command("spawn_smoke")]
        private static void SpawnSmoke()
        {
            Vector2 position = GameplayScene.MouseWorldPosition;
            GameplayScene.Instance.SmokeManager.AddSmoke(position);
        }

        [Command("spawn_settler")]
        private static void SpawnSettler()
        {
            int x = GameplayScene.MouseTile.X;
            int y = GameplayScene.MouseTile.Y;
            SettlerInfo settlerInfo = SettlerGenerator.GenerateSettler();
            GameplayScene.Instance.SpawnSettler(x, y, settlerInfo, GameplayScene.WorldManager.NewSettlerFoodRationFilters);
        }

        [Command("spawn_animal")]
        private static void SpawnAnimal(string animalName)
        {
            int x = GameplayScene.MouseTile.X;
            int y = GameplayScene.MouseTile.Y;
            AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(animalName);
            GameplayScene.Instance.SpawnAnimal(x, y, animalTemplate, animalTemplate.DaysUntilAging);
        }

        [Command("add_item")]
        private static void AddItem(string itemName, int weight, float durabilityPercent = 1f)
        {
            if (ItemDatabase.Items.TryGetValue(itemName, out Item item))
            {
                durabilityPercent = Math.Clamp(durabilityPercent, 0f, 1f);

                float realDurability = item.Durability * durabilityPercent;
                GameplayScene.MouseTile.Inventory.AddCargo(item, weight, realDurability);
            }
        }

        [Command("show_item")]
        private static void ShowItem(string itemName)
        {
            Item item;
            if (ItemDatabase.Items.TryGetValue(itemName, out item))
            {
                GameplayScene.WorldManager.ShowItem(item);
            }
        }

        [Command("hide_item")]
        private static void HideItem(string itemName)
        {
            Item item;
            if (ItemDatabase.Items.TryGetValue(itemName, out item))
            {
                GameplayScene.WorldManager.HideItem(item);
            }
        }

        [Command("set_new_settler_food_ration")]
        private static void SetNewSettlerFoodRationFilter(string itemName, bool value)
        {
            Item item;
            if (ItemDatabase.Items.TryGetValue(itemName, out item))
            {
                if (GameplayScene.WorldManager.NewSettlerFoodRationFilters.ContainsKey(item) == false)
                    return;

                GameplayScene.WorldManager.NewSettlerFoodRationFilters[item] = value;
            }
        }

        [Command("limit_item")]
        private static void LimitItem(string itemName, int limit)
        {
            Item item;
            if(ItemDatabase.Items.TryGetValue(itemName, out item))
            {
                GameplayScene.Instance.ResourcesLimitManager.SetItemLimit(item, limit);
            }
        }

        [Command("gbtac")]
        private static void GiveBirthToAChild()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalCmp animal = entity?.Get<AnimalCmp>();
            if (animal != null)
            {
                animal.GiveBirthToAChild();
            }
        }
        
        [Command("domesticate")]
        private static void Domesticate()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalCmp animal = entity?.Get<AnimalCmp>();
            if (animal != null)
            {
                animal.TurnToDomesticated();
            }
        }
        
        [Command("fertilize")]
        private static void Fertilize()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalCmp animal = entity?.Get<AnimalCmp>();
            if (animal != null)
            {
                animal.Fertilize(null);
            }
        }

        [Command("hit_lightning")]
        private static void HitLightning()
        {
            Tile tile = GameplayScene.MouseTile;
            GameplayScene.Instance.StormManager.HitLightning(tile);
        }

        [Command("trigger_notif")]
        private static void TriggerNotif(NotificationLevel notificationLevel, string message)
        {
            GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                                        .AddNotification(message, notificationLevel);
        }

        [Command("unlock_achv")]
        private static void UnlockAchievement(AchievementId id)
        {
            GameplayScene.Instance.AchievementManager.UnlockAchievement(id);
        }

        [Command("set_game_overlay")]
        private static void SetGameOverlay(GameOverlayType overlayType)
        {
            GameplayScene.Instance.OverlayManager.SetOverlay(overlayType);
        }
    }

}
