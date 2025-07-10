using Microsoft.Xna.Framework;
using Technolithic.UI.Prefabs.BuildingRecipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace Technolithic
{
    public class UIRootNodeScript : MComponent
    {
        private ActionPanelUI actionPanelUI;
        private GameMenuUI gameMenuUI;
        private LaborPriorityUI laborPriorityUI;
        private CommandLineUI commandLineUI;
        private RationUI rationUI;
        private ResourcesLimitUI resourcesLimitUI;
        private ResourcesUI resourcesUI;
        private AchievementsUI achievementsUI;
        private TechTreeUI techTreeUI;
        private NomadsUI nomadsUI;
        private TradingUI tradingUI;
        private BuildingUI buildingUI;
        private BuildingPanelUI buildingPanelUI;
        private CrafterUI crafterUI;
        private StorageUI storageUI;
        private AnimalPenUI animalPenUI;
        private BuildingsListUI buildingListUI;
        private HutUI hutUI;
        private GateUI gateUI;
        private CreatureUI creatureUI;
        private StatusEffectsUI statusEffectsUI;
        private AssignPetUI attachPetUI;
        private AssignHutUI assignHutUI;
        private ItemsStackUI itemsStackUI;
        private BuildRecipeUI buildingRecipeUI;
        private PriorityUI priorityUI;
        private TimeControllerUI timeControllerUI;
        private WorldStateUI worldStateUI;
        private LevelUI levelUI;
        private UnitCommandUI unitCommandUI;
        private RenameCreatureUI renameCreatureUI;

        public NotificationsUI NotificationsUI { get; private set; }

        private List<BigButton> uiButtons = new List<BigButton>();

        private bool showDebugInfo = false;
        private MyText debugInfoText;

        private MNode mainPanel;
        private List<MNode> entityPanels = new List<MNode>();

        private int lastGameSpeed;
        private MyText gameVersionText;

        private bool entityPanelsInversed = false;

        public UIRootNodeScript() : base(true, false)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            actionPanelUI = (ActionPanelUI)ParentNode.GetChildByName("ActionPanel");
            buildingPanelUI = (BuildingPanelUI)ParentNode.GetChildByName("BuildingPanel");

            gameMenuUI = (GameMenuUI)ParentNode.GetChildByName("GameMenu");
            laborPriorityUI = (LaborPriorityUI)ParentNode.GetChildByName("LaborPriority");
            commandLineUI = (CommandLineUI)ParentNode.GetChildByName("CommandLine");
            rationUI = (RationUI)ParentNode.GetChildByName("Ration");
            resourcesLimitUI = (ResourcesLimitUI)ParentNode.GetChildByName("ResourcesLimit");
            resourcesUI = (ResourcesUI)ParentNode.GetChildByName("Resources");
            achievementsUI = (AchievementsUI)ParentNode.GetChildByName("Achievements");
            techTreeUI = (TechTreeUI)ParentNode.GetChildByName("TechTree");

            nomadsUI = (NomadsUI)ParentNode.GetChildByName("Nomads");

            tradingUI = (TradingUI)ParentNode.GetChildByName("Trading");

            buildingUI = (BuildingUI)ParentNode.GetChildByName("Building");

            NotificationsUI = (NotificationsUI)ParentNode.GetChildByName("Notifications");

            crafterUI = (CrafterUI)ParentNode.GetChildByName("Crafter");

            storageUI = (StorageUI)ParentNode.GetChildByName("Storage");

            animalPenUI = (AnimalPenUI)ParentNode.GetChildByName("AnimalPen");

            buildingListUI = (BuildingsListUI)ParentNode.GetChildByName("BuildingList");

            hutUI = (HutUI)ParentNode.GetChildByName("Hut");

            gateUI = (GateUI)ParentNode.GetChildByName("Gate");

            creatureUI = (CreatureUI)ParentNode.GetChildByName("Creature");

            statusEffectsUI = (StatusEffectsUI)ParentNode.GetChildByName("StatusEffects");

            attachPetUI = (AssignPetUI)ParentNode.GetChildByName("AttachPet");

            assignHutUI = (AssignHutUI)ParentNode.GetChildByName("AssignHut");

            itemsStackUI = (ItemsStackUI)ParentNode.GetChildByName("ItemsStack");

            buildingRecipeUI = (BuildRecipeUI)ParentNode.GetChildByName("BuildingRecipe");

            priorityUI = (PriorityUI)ParentNode.GetChildByName("Priority");

            timeControllerUI = (TimeControllerUI)ParentNode.GetChildByName("TimeController");

            worldStateUI = (WorldStateUI)ParentNode.GetChildByName("WorldState");

            debugInfoText = new MyText(ParentNode.Scene);
            debugInfoText.X = worldStateUI.X + worldStateUI.Width + 5;
            debugInfoText.Y = 5;
            debugInfoText.Outlined = true;

            levelUI = (LevelUI)ParentNode.GetChildByName("Level");

            unitCommandUI = new UnitCommandUI(ParentNode.Scene);
            unitCommandUI.Active = false;
            ParentNode.AddChildNode(unitCommandUI);

            renameCreatureUI = new RenameCreatureUI(ParentNode.Scene);
            renameCreatureUI.Active = false;
            ParentNode.AddChildNode(renameCreatureUI);

            var laborPriorityButton = (BigButton)ParentNode.GetChildByName("LaborPriorityButton");
            laborPriorityButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenLaborPriorityUI);
            laborPriorityButton.Tooltips = Localization.GetLocalizedText("priority") + " [P]";
            uiButtons.Add(laborPriorityButton);

            var rationButton = (BigButton)ParentNode.GetChildByName("RationButton");
            rationButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenRationUI);
            rationButton.Tooltips = Localization.GetLocalizedText("ration") + " [O]";
            uiButtons.Add(rationButton);

            var resourcesLimitButton = (BigButton)ParentNode.GetChildByName("ResourcesLimitButton");
            resourcesLimitButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenResourcesLimitUI);
            resourcesLimitButton.Tooltips = Localization.GetLocalizedText("production_limit") + " [L]";
            uiButtons.Add(resourcesLimitButton);

            var resourcesButton = (BigButton)ParentNode.GetChildByName("ResourcesButton");
            resourcesButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenResourcesUI);
            resourcesButton.Tooltips = Localization.GetLocalizedText("resources") + " [I]";
            uiButtons.Add(resourcesButton);

            var achievementsButton = (BigButton)ParentNode.GetChildByName("AchievementsButton");
            achievementsButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenAchievementsUI);
            achievementsButton.Tooltips = Localization.GetLocalizedText("achievements") + " [?]";
            uiButtons.Add(achievementsButton);

            gameVersionText = new MyText(ParentNode.Scene);
            gameVersionText.Text = Engine.GAME_NAME + " " + Engine.VERSION_STRING;
            gameVersionText.Outlined = true;
            gameVersionText.X = Engine.Width / 2;
            gameVersionText.Y = Engine.Height - 40;
            ParentNode.AddChildNode(gameVersionText);

            Engine.Instance.Window.ClientSizeChanged += OnClientSizeChanged;
            ParentNode.Scene.SceneEnded += OnSceneEnded;
        }

        private void OnSceneEnded(Scene scene)
        {
            Engine.Instance.Window.ClientSizeChanged -= OnClientSizeChanged;
            ParentNode.Scene.SceneEnded -= OnSceneEnded;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            gameVersionText.X = Engine.Width / 2;
            gameVersionText.Y = Engine.Height - 40;

            if (mainPanel != null && mainPanel.Active)
            {
                mainPanel.X = Engine.Width / 2 - mainPanel.Width / 2;
                mainPanel.Y = Engine.Height / 2 - mainPanel.Height / 2;
            }

            for (int i = 0; i < entityPanels.Count; i++)
            {
                MNode node = entityPanels[i];
                MNode lastNode = null;

                if (i - 1 >= 0)
                {
                    lastNode = entityPanels[i - 1];
                }

                if (entityPanelsInversed)
                {
                    if (lastNode == null)
                    {
                        node.X = 5;
                    }
                    else
                    {
                        node.X = lastNode.LocalX + lastNode.Width;
                    }
                }
                else
                {
                    if (lastNode == null)
                    {
                        node.X = Engine.Width - node.Width - 5;
                    }
                    else
                    {
                        node.X = lastNode.LocalX - node.Width;
                    }
                }
            }

            if (entityPanels.Count > 0)
            {
                foreach (var node in entityPanels)
                {
                    node.Y = (Engine.Height - node.Height) - 64;
                }
            }
        }

        private Tile lastTile;

        public override void Update(int mouseX, int mouseY)
        {
            Tile currentTile = GameplayScene.MouseTile;

            if (lastTile != currentTile)
            {
                lastTile = currentTile;

                if (showDebugInfo)
                {
                    debugInfoText.Text = lastTile.GetInformation() + $"\nFPS: {(int)Engine.Instance.FrameCounter.AverageFramesPerSecond}\n" +
                        $"Animals to domesticate amount: {GameplayScene.WorldManager.AnimalsToDomesticate.Count}";
                }
            }

            if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                showDebugInfo = !showDebugInfo;

                if (showDebugInfo)
                {
                    ParentNode.AddChildNode(debugInfoText);
                }
                else
                {
                    ParentNode.RemoveChild(debugInfoText);
                }
            }

            if (mainPanel == null)
            {
                if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.T))
                {
                    OpenTechTreeUI(false, null);
                }

                if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.O))
                {
                    OpenRationUI(false, null);
                }

                if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.P))
                {
                    OpenLaborPriorityUI(false, null);
                }

                if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    OpenResourcesLimitUI(false, null);
                }

                if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.I))
                {
                    OpenResourcesUI(false, null);
                }

                if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.OemTilde))
                {
                    OpenCommandLineUI();
                }
            }

            if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

                if (mainPanel != null)
                {
                    // TODO: косыль
                    if (mainPanel is NomadsUI)
                        return;

                    CloseMainPanel();
                }
                else if (entityPanels.Count != 0)
                {
                    CloseEntityPanel();
                }
                else
                {
                    OpenGameMenu();
                }
            }
        }

        public override void Render()
        {

        }

        public bool IsUnitCommandUIOpened()
        {
            return unitCommandUI.Active;
        }

        public void OpenUnitCommandUI(int x, int y, Tile tile)
        {
            unitCommandUI.GetComponent<UnitCommandUIScript>().Open(tile);
            unitCommandUI.Active = true;
            unitCommandUI.X = x - unitCommandUI.Width / 2;
            unitCommandUI.Y = y - unitCommandUI.Height - (unitCommandUI.Height / 2);
        }

        public void CloseUnitCommandUI()
        {
            unitCommandUI.Active = false;
        }

        public void OpenRenameCreatureUI(CreatureCmp creature)
        {
            renameCreatureUI.Open(creature);

            OpenMainPanel(renameCreatureUI);
        }

        public void OpenNomadsUI()
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            nomadsUI.GetComponent<NomadsUIScript>().Open();

            OpenMainPanel(nomadsUI);
        }

        public void OpenTradingUI(TradingPostBuildingCmp tradingPost)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            tradingUI.GetComponent<TradingUIScript>().Open(tradingPost);

            OpenMainPanel(tradingUI);
        }

        public void OpenGameMenu()
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            gameMenuUI.GetComponent<GameMenuScript>().Open();

            OpenMainPanel(gameMenuUI);
        }

        public void OpenTechTreeUI(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            techTreeUI.GetComponent<TechTreeUIScript>().Open();

            OpenMainPanel(techTreeUI);
        }

        private void OpenLaborPriorityUI(bool value, ButtonScript button)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            laborPriorityUI.GetComponent<LaborPriorityUIScript>().Open();

            OpenMainPanel(laborPriorityUI);
        }

        private void OpenCommandLineUI()
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            OpenMainPanel(commandLineUI);
        }

        private void OpenRationUI(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            rationUI.GetComponent<RationUIScript>().Open();

            OpenMainPanel(rationUI);
        }

        private void OpenResourcesLimitUI(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            resourcesLimitUI.GetComponent<ResourcesLimitUIScript>().Open(GameplayScene.Instance.ResourcesLimitManager);

            OpenMainPanel(resourcesLimitUI);
        }

        private void OpenResourcesUI(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            resourcesUI.GetComponent<ResourcesUIScript>().Open();

            OpenMainPanel(resourcesUI);
        }

        private void OpenAchievementsUI(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            achievementsUI.GetComponent<AchievementsUIScript>().Open();

            OpenMainPanel(achievementsUI);
        }

        public void OpenPriorityUI(StorageBuildingCmp storage)
        {
            priorityUI.GetComponent<PriorityUIScript>().Open(storage);
            OpenEntityPanel(priorityUI);
        }

        public void OpenBuildingTemplateRecipeUI(BuildingTemplate buildingTemplate)
        {
            buildingRecipeUI.GetComponent<BuildRecipeScript>().SetBuildingTemplate(buildingTemplate);
            OpenEntityPanel(buildingRecipeUI, true);
        }

        public void OpenItemStackUI(Tile tile)
        {
            CloseEntityPanel();

            itemsStackUI.GetComponent<ItemsStackUIScript>().SetItems(tile);
            OpenEntityPanel(itemsStackUI);
        }

        public void OpenBuildingListUI(BuildingCategory category, string name)
        {
            CloseEntityPanel();

            buildingListUI.GetComponent<BuildingListUIScript>().Open(category, name);
            OpenEntityPanel(buildingListUI, true);
        }

        public void OpenBuildingUI(BuildingCmp building, InteractionsDatabase interactionsDatabase, ProgressTree progressTree)
        {
            CloseEntityPanel();

            buildingUI.GetComponent<BuildingUIScript>().SetBuilding(building, interactionsDatabase, progressTree);
            OpenEntityPanel(buildingUI);

            if (building.IsBuilt)
            {
                if (building is CrafterBuildingCmp)
                {
                    crafterUI.GetComponent<CrafterUIScript>().SetCrafter((CrafterBuildingCmp)building);
                    OpenEntityPanel(crafterUI);
                }
                else if (building is StorageBuildingCmp && building.BuildingTemplate.Storage.IsEditable)
                {
                    storageUI.GetComponent<StorageUIScript>().SetStockpile((StorageBuildingCmp)building);
                    OpenEntityPanel(storageUI);
                }
                else if(building is AnimalPenBuildingCmp)
                {
                    animalPenUI.GetComponent<AnimalPenUIScript>().SetAnimalPen((AnimalPenBuildingCmp)building);
                    OpenEntityPanel(animalPenUI);
                }
                else if (building is HutBuildingCmp)
                {
                    hutUI.GetComponent<HutUIScript>().SetHut((HutBuildingCmp)building);
                    OpenEntityPanel(hutUI);
                }
                else if (building is GateCmp)
                {
                    gateUI.GetComponent<GateUIScript>().Open((GateCmp)building);
                    OpenEntityPanel(gateUI);
                }
            }
        }

        public void OpenCreatureUI(CreatureCmp creature)
        {
            CloseEntityPanel();

            creatureUI.GetComponent<CreatureUIScript>().SetCreature(creature);
            OpenEntityPanel(creatureUI);

            statusEffectsUI.GetComponent<StatusEffectsUIScript>().Open(creature.StatusEffectsManager);
            OpenEntityPanel(statusEffectsUI);
        }

        public void OpenAttachPetUI(CreatureCmp parentCreature)
        {
            attachPetUI.GetComponent<AssignPetUIScript>().Open(parentCreature);
            OpenEntityPanel(attachPetUI);
        }

        public void OpenAssignHutUI(HutBuildingCmp hut, int slotIndex)
        {
            assignHutUI.GetComponent<AssignHutUIScript>().Open(hut, slotIndex);
            OpenEntityPanel(assignHutUI);
        }

        public void CloseActionPanel()
        {
            actionPanelUI.GetComponent<ActionPanelScript>().UnselectAll();
        }

        public void CloseAll()
        {
            actionPanelUI.GetComponent<ActionPanelScript>().UnselectAll();

            CloseMainPanel();
            CloseEntityPanel();
        }

        public void OpenMainPanel(MNode node)
        {
            lastGameSpeed = Engine.GameSpeed;
            Engine.GameSpeed = 0;

            GameplayScene.OnGameMenu = true;

            mainPanel = node;
            mainPanel.Active = true;

            mainPanel.X = Engine.Width / 2 - mainPanel.Width / 2;
            mainPanel.Y = Engine.Height / 2 - mainPanel.Height / 2;

            levelUI.Active = false;
            worldStateUI.Active = false;
            foreach(var uiButton in uiButtons)
            {
                uiButton.Active = false;
            }
            timeControllerUI.Active = false;
            actionPanelUI.Active = false;
            buildingPanelUI.Active = false;
            ParentNode.GetChildByName("Notifications").Active = false;
            ParentNode.GetChildByName("ResourcesCount").Active = false;

            foreach (var panel in entityPanels)
            {
                panel.Active = false;
            }
        }

        public void CloseMainPanel()
        {
            if (mainPanel != null)
            {
                mainPanel.Active = false;
                mainPanel = null;

                Engine.GameSpeed = lastGameSpeed;

                GameplayScene.OnGameMenu = false;

                levelUI.Active = true;
                worldStateUI.Active = true;
                foreach (var uiButton in uiButtons)
                {
                    uiButton.Active = true;
                }
                timeControllerUI.Active = true;
                actionPanelUI.Active = true;
                buildingPanelUI.Active = true;
                ParentNode.GetChildByName("Notifications").Active = true;
                ParentNode.GetChildByName("ResourcesCount").Active = true;

                foreach (var panel in entityPanels)
                {
                    panel.Active = true;
                }
            }
        }

        public void OpenEntityPanel(MNode node, bool inverse = false)
        {
            entityPanelsInversed = inverse;
            MNode lastNode = null;

            if (entityPanels.Count > 0)
                lastNode = entityPanels[entityPanels.Count - 1];

            if (entityPanels.Contains(node))
                return;

            if (lastNode != null)
            {
                if (inverse)
                    node.X = lastNode.LocalX + lastNode.Width;
                else
                    node.X = lastNode.LocalX - node.Width;
            }
            else
            {
                if (inverse)
                    node.X = 5;
                else
                    node.X = Engine.Width - (node.Width + 5);
            }

            node.Y = (Engine.Height - node.Height) - 64;

            node.Active = true;

            entityPanels.Add(node);
        }

        public void CloseEntityPanel()
        {
            foreach (var node in entityPanels)
            {
                node.Active = false;

                if (node.HasComponent<BuildingUIScript>())
                {
                    node.GetComponent<BuildingUIScript>().Close();
                }
                else if (node.HasComponent<HutUIScript>())
                {
                    node.GetComponent<HutUIScript>().Close();
                }
            }

            entityPanels.Clear();
        }

    }
}
