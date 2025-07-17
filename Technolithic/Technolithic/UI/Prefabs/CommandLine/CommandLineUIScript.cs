using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Penumbra;

namespace Technolithic
{
    public class CommandLineUIScript : MScript
    {

        private MTextInputScript textInput;

        private Dictionary<string, MethodInfo> commands;

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
            commands = new Dictionary<string, MethodInfo>();

            lastEnteredCommands = new List<string>();

            GatherCommands();

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

                    string name = GetCommandName(enteredCommand);
                    string[] args = GetCommandArgs(enteredCommand);

                    ExecuteCommand(name, args);

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

        private void ExecuteCommand(string name, string[] args)
        {
            if (commands.ContainsKey(name) == false)
                return;

            MethodInfo method = commands[name];

            object[] parameters = null;

            if (args != null && args.Length > 0)
            {
                parameters = new object[args.Length];

                int paramNum = 0;
                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType == typeof(int))
                    {
                        parameters[paramNum] = int.Parse(args[paramNum]);
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        parameters[paramNum] = args[paramNum];
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        parameters[paramNum] = bool.Parse(args[paramNum]);
                    }

                    paramNum++;
                }
            }

            method.Invoke(null, parameters);
        }

        private string GetCommandName(string command)
        {
            string[] splitted = command.Split(new char[] { ' ' });

            return splitted[0];
        }

        private string[] GetCommandArgs(string command)
        {
            string[] splitted = command.Split(new char[] { ' ' });

            string[] args = null;

            if (splitted.Length > 1)
            {
                args = new string[splitted.Length - 1];

                for (int i = 1; i < splitted.Length; i++)
                {
                    args[i - 1] = splitted[i];
                }
            }

            return args;
        }

        private void GatherCommands()
        {
            var methods = Assembly.GetCallingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttributes(typeof(Command), false).Length > 0)
                .ToArray();

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                Command command = (Command)method.GetCustomAttribute(typeof(Command));

                commands.Add(command.Name, method);
            }
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
        private static void SetAttribute(string attributeTypeString, int value)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                AttributeType attributeType = Utils.ParseEnum<AttributeType>(attributeTypeString);
                var attribute = creature.CreatureStats.GetAttribute(attributeType);
                if (attribute != null)
                {
                    attribute.CurrentValue = value;
                }
            }
        }

        [Command("add_status_effect")]
        private static void AddStatusEffect(string statusEffectIdString)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                StatusEffectId statusEffectId = Utils.ParseEnum<StatusEffectId>(statusEffectIdString);
                creature.StatusEffectsManager.AddStatusEffect(statusEffectId);
            }
        }

        [Command("remove_status_effect")]
        private static void RemoveStatusEffect(string statusEffectIdString)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            CreatureCmp creature = entity?.Get<CreatureCmp>();
            if (creature != null)
            {
                StatusEffectId statusEffectId = Utils.ParseEnum<StatusEffectId>(statusEffectIdString);
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

        [Command("show_irrigated_tiles")]
        private static void SetShowIrrigatedTiles(bool value)
        {
            GameplayScene.ShowIrrigatedTiles = value;
        }

        [Command("show_ilm_tiles")]
        private static void SetShowIlmTiles(bool value)
        {
            GameplayScene.ShowIlluminatedTiles = value;
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
        private static void AddItem(string itemName, int weight)
        {
            Item item;
            if(ItemDatabase.Items.TryGetValue(itemName, out item))
            {
                GameplayScene.MouseTile.Inventory.AddCargo(item, weight);
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

        [Command("fill_beehive")]
        private static void FillBeehive()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            BeeHiveBuildingCmp beeHiveBuildingCmp = entity?.Get<BeeHiveBuildingCmp>();
            if (beeHiveBuildingCmp != null)
            {
                beeHiveBuildingCmp.SetProgress(100);
            }
        }

        [Command("fill_animal_product")]
        private static void FillAnimalProduct()
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            AnimalCmp animalCmp = entity?.Get<AnimalCmp>();
            if (animalCmp != null)
            {
                animalCmp.ProductReadyPercent = 100;
            }
        }

        [Command("set_priority")]
        private static void SetPriority(int value)
        {
            Entity entity = GameplayScene.WorldManager.GetSelectedEntity();
            Interactable interactable = entity?.Get<Interactable>();
            if (interactable != null)
            {
                interactable.Priority = value;
            }
        }

        [Command("hit_lightning")]
        private static void HitLightning()
        {
            Tile tile = GameplayScene.MouseTile;
            GameplayScene.Instance.StormManager.HitLightning(tile);
        }

        [Command("trigger_notif")]
        private static void TriggerNotif(string notifLevelString, string message)
        {
            NotificationLevel notificationLevel = Utils.ParseEnum<NotificationLevel>(notifLevelString);

            GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                                        .AddNotification(message, notificationLevel);
        }

        [Command("unlock_achv")]
        private static void UnlockAchievement(string achievementIdString)
        {
            try
            {
                AchievementId id = Utils.ParseEnum<AchievementId>(achievementIdString);

                GameplayScene.Instance.AchievementManager.UnlockAchievement(id);
            }
            catch { }
        }
    }

}
