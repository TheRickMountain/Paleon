using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingUIScript : MScript
    {
        private Dictionary<Item, MNode> fuelNodes;
        private Dictionary<MNode, Item> nodesFuels;

        private BuildingCmp selectedBuilding;
        private Interactable selectedInteractable;
        private MyPanelUI panel;

        private BigButton cancelButton;
        private BigButton cutChopButton;
        private BigButton cutChopCompletelyButton;
        private BigButton cloneButton;
        private BigButton irrigateButton;
        private BigButton fertilizeButton;
        private BigButton copySettingsButton;
        private BigButton gatherButton;
        private BigButton mineButton;
        private BigButton priorityButton;
        private BigButton cleanUpManureButton;
        private BigButton autoMineSpawnedDepositsButton;
        private BigButton tradeButton;
        private MTextButtonUI returnHomeButton;

        private Dictionary<Tab, MNode> tabNodes = new Dictionary<Tab, MNode>();

        private RichTextUI infoText;

        private Dictionary<InteractionType, BigButton> interactionButtonDict = new();

        public BuildingUIScript() : base(true)
        {
            fuelNodes = new Dictionary<Item, MNode>();
            nodesFuels = new Dictionary<MNode, Item>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            panel = (MyPanelUI)ParentNode;

            cancelButton = new BigButton(ParentNode.Scene, ResourceManager.CancelIcon, true);
            cancelButton.GetComponent<ButtonScript>().AddOnClickedCallback(CancelBuilding);
            cancelButton.Tooltips = Localization.GetLocalizedText("cancel");

            cutChopButton = new BigButton(ParentNode.Scene, RenderManager.Pixel, true);
            cutChopButton.GetComponent<ButtonScript>().AddOnClickedCallback(HarvestBuilding);

            cutChopCompletelyButton = new BigButton(ParentNode.Scene, RenderManager.Pixel, true);
            cutChopCompletelyButton.GetComponent<ButtonScript>().AddOnClickedCallback(ChopBuilding);

            cloneButton = new BigButton(ParentNode.Scene, RenderManager.Pixel, false);
            cloneButton.Tooltips = Localization.GetLocalizedText("clone") + " [Q]";
            cloneButton.GetComponent<ButtonScript>().AddOnClickedCallback(CloneBuilding);

            irrigateButton = new BigButton(ParentNode.Scene, ResourceManager.IrrigationIcon, true);
            irrigateButton.GetComponent<ButtonScript>().AddOnClickedCallback(IrrigateBuilding);
            irrigateButton.Tooltips = Localization.GetLocalizedText("irrigate");

            fertilizeButton = new BigButton(ParentNode.Scene, ResourceManager.FertilizeIcon, true);
            fertilizeButton.GetComponent<ButtonScript>().AddOnClickedCallback(FertilizeBuilding);
            fertilizeButton.Tooltips = Localization.GetLocalizedText("fertilize");

            copySettingsButton = new BigButton(ParentNode.Scene, ResourceManager.CopyIcon, false);
            copySettingsButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnCopySettingsButtonPressedCallback);
            copySettingsButton.Tooltips = Localization.GetLocalizedText("copy_settings");

            mineButton = new BigButton(ParentNode.Scene, ResourceManager.MineIcon, true);
            mineButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnMineButtonPressedCallback);
            mineButton.Tooltips = Localization.GetLocalizedText("mine");

            cleanUpManureButton = new BigButton(ParentNode.Scene, ItemDatabase.GetItemByName("manure").Icon, true);
            cleanUpManureButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnCleanUpManureButtonPressedCallback);
            cleanUpManureButton.Tooltips = Localization.GetLocalizedText("clean_up_manure");

            autoMineSpawnedDepositsButton = new BigButton(ParentNode.Scene, ResourceManager.GetTexture("ui").GetSubtexture(240, 0, 16, 16), true, false);
            autoMineSpawnedDepositsButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnAutoMineSpawnedDepositsButtonPressedCallback);
            autoMineSpawnedDepositsButton.Tooltips = Localization.GetLocalizedText("automatically_mine_spawned_deposits");

            tradeButton = new BigButton(ParentNode.Scene, ResourceManager.TradingIcon, false);
            tradeButton.Tooltips = Localization.GetLocalizedText("trade");
            tradeButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnTradeButtonPressed);

            priorityButton = new BigButton(ParentNode.Scene, ResourceManager.PriorityIcon, false, false);
            priorityButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnPriorityButtonPressedCallback);
            priorityButton.Tooltips = Localization.GetLocalizedText("priority");

            returnHomeButton = new MTextButtonUI(ParentNode.Scene);
            returnHomeButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnReturnHomeButtonPressedCallback);
            returnHomeButton.Width = ParentNode.Width - 16;
            returnHomeButton.Height = 48;
            returnHomeButton.Name = "ReturnHomeButton";
            (returnHomeButton.GetChildByName("Text") as MyText).Text = Localization.GetLocalizedText("return_to_the_future");
            returnHomeButton.CenterText();

            gatherButton = new BigButton(ParentNode.Scene, ResourceManager.GatherIcon, true);
            gatherButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnGatherButtonPressedCallback);
            gatherButton.Tooltips = Localization.GetLocalizedText("gather");

            ParentNode.GetChildByName("StatsTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("InventoryTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("FuelTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);

            tabNodes.Add((Tab)ParentNode.GetChildByName("StatsTab"), ParentNode.GetChildByName("StatsListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("InventoryTab"), ParentNode.GetChildByName("InventoryListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("FuelTab"), ParentNode.GetChildByName("FuelListView"));

            infoText = new RichTextUI(ParentNode.Scene);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if(panel.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                if (selectedBuilding.BuildingTemplate.Cloneable &&
                GameplayScene.Instance.ProgressTree.IsBuildingUnlocked(selectedBuilding.BuildingTemplate))
                {
                    CloneBuilding(true, null);
                }
            }
        }

        public void SetBuilding(BuildingCmp building, InteractionsDatabase interactionsDatabase, ProgressTree progressTree)
        {
            selectedBuilding = building;

            selectedInteractable = building;

            selectedBuilding.OnBuildingCanceledCallback += CloseUI;
            selectedBuilding.OnBuildingCompletedCallback += CloseUI;
            selectedBuilding.OnBuildingDestructedCallback += CloseUI;

            ((RichTextUI)panel.GetChildByName("Label")).Text = building.BuildingTemplate.Name;

            UpdateStatsListView(building);

            (ParentNode.GetChildByName("InventoryListView") as InventoryListViewUI).SetInventory(selectedBuilding.Inventory);

            if (building.BuildingTemplate.FuelConsumer != null && building.IsBuilt)
            {
                ParentNode.GetChildByName("FuelTab").Active = true;

                UpdateFuelListView(building);
            }
            else
            {
                ParentNode.GetChildByName("FuelTab").Active = false;
            }

            SetTab(true, ParentNode.GetChildByName("StatsTab").GetComponent<ButtonScript>());

            UpdateButtons();

            // TODO: refactoring required
            foreach(InteractionType interactionType in selectedInteractable.AvailableInteractions)
            {
                BigButton interactionButton;

                InteractionData interactionData = interactionsDatabase.GetInteractionData(interactionType);

                if (interactionButtonDict.ContainsKey(interactionType) == false)
                {
                    interactionButton = new BigButton(ParentNode.Scene, interactionData.Icon, true);
                    interactionButtonDict[interactionType] = interactionButton;
                    interactionButton.ButtonScript.Pressed += Interactable_InteractionButton_Pressed;
                    interactionButton.SetMetadata("interaction_data", interactionData);
                }

                interactionButton = interactionButtonDict[interactionType];
                interactionButton.SetMetadata("interactable", selectedInteractable);
                interactionButton.Tooltips = "";

                Technology requiredTechnology = TechnologyDatabase.GetTechnologyThatUnlocksInteraction(interactionType);
                if (requiredTechnology != null && progressTree.IsTechnologyUnlocked(requiredTechnology) == false)
                {
                    interactionButton.ButtonScript.IsDisabled = true;
                    interactionButton.Tooltips = $"{Localization.GetLocalizedText($"required_technology_x", requiredTechnology.Name).Paint(Color.Yellow)}\n";
                }
                else
                {
                    interactionButton.ButtonScript.IsDisabled = false;
                    interactionButton.ButtonScript.IsSelected = selectedInteractable.IsInteractionMarked(interactionType);
                }

                interactionButton.Tooltips += GenerateInteractionTooltip(building, interactionData);

                ListViewUIScript buttonsListView = ParentNode.GetChildByName("ButtonsListView").GetComponent<ListViewUIScript>();
                buttonsListView.AddItem(interactionButton);
            }
        }

        public void CloseUI(BuildingCmp buildingCmp)
        {
            GameplayScene.UIRootNodeScript.CloseEntityPanel();
        }

        public void Close()
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.OnBuildingCanceledCallback -= CloseUI;
                selectedBuilding.OnBuildingCompletedCallback -= CloseUI;
                selectedBuilding.OnBuildingDestructedCallback -= CloseUI;
            }
        }

        public void OnCopySettingsButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.BuildingSaveDataForCopy = selectedBuilding.GetSaveData();
            GameplayScene.WorldManager.SetMyAction(MyAction.CopySettings, ResourceManager.CopyIcon);
        }

        public void OnMineButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            DepositCmp depositCmp = selectedBuilding as DepositCmp;
            depositCmp.IsMarkedToObtain = !depositCmp.IsMarkedToObtain;
        }

        public void OnCleanUpManureButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            AnimalPenBuildingCmp animalPen = selectedBuilding as AnimalPenBuildingCmp;
            animalPen.IsFlaggedToCleanManure = !animalPen.IsFlaggedToCleanManure;
        }

        private void OnTradeButtonPressed(bool value, ButtonScript script)
        {
            GameplayScene.UIRootNodeScript.OpenTradingUI(selectedBuilding as TradingPostBuildingCmp);
        }

        public void OnAutoMineSpawnedDepositsButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            MineBuildingCmp mineBuilding = selectedBuilding as MineBuildingCmp;
            mineBuilding.AutoMineSpawnedDeposits = !mineBuilding.AutoMineSpawnedDeposits;
        }

        public void OnPriorityButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            StorageBuildingCmp storage = selectedBuilding as StorageBuildingCmp;
            GameplayScene.UIRootNodeScript.OpenPriorityUI(storage);
        }

        public void OnReturnHomeButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            TimeMachine timeMachine = selectedBuilding as TimeMachine;
            timeMachine.ReturnHome = true;
        }

        public void OnGatherButtonPressedCallback(bool value, ButtonScript buttonScript)
        {
            if (selectedBuilding is DepositCmp)
            {
                DepositCmp depositCmp = selectedBuilding as DepositCmp;
                depositCmp.IsMarkedToObtain = !depositCmp.IsMarkedToObtain;
            }
        }

        public void FertilizeBuilding(bool value, ButtonScript sender)
        {
            FarmPlot wildFarmPlot = selectedBuilding as FarmPlot;
            wildFarmPlot.Fertilize = !wildFarmPlot.Fertilize;

            UpdateButtons();
        }

        public void IrrigateBuilding(bool value, ButtonScript sender)
        {
            FarmPlot wildFarmPlot = selectedBuilding as FarmPlot;
            wildFarmPlot.Irrigate = !wildFarmPlot.Irrigate;

            UpdateButtons();
        }

        public void CloneBuilding(bool value, ButtonScript sender)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.Build, null);
            GameplayScene.WorldManager.SetBuilding(selectedBuilding.BuildingTemplate);
            CloseUI(selectedBuilding);
        }

        public void ChopBuilding(bool value, ButtonScript sender)
        {
            FarmPlot wildFarmPlot = selectedBuilding as FarmPlot;
            wildFarmPlot.Chop = !wildFarmPlot.Chop;

            UpdateButtons();
        }

        public void HarvestBuilding(bool value, ButtonScript sender)
        {
            FarmPlot wildFarmPlot = selectedBuilding as FarmPlot;
            wildFarmPlot.Harvest = !wildFarmPlot.Harvest;

            UpdateButtons();
        }

        public void CancelBuilding(bool value, ButtonScript sender)
        {
            selectedBuilding.CancelBuilding();
            CloseUI(selectedBuilding);
        }

        private void UpdateStatsListView(BuildingCmp building)
        {
            MNode listView = ParentNode.GetChildByName("StatsListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            infoText.Text = building.GetInformation();
            infoText.Height = infoText.TextHeight;
            listViewScript.AddItem(infoText);
        }

        private void UpdateFuelListView(BuildingCmp buildingCmp)
        {
            MNode listView = ParentNode.GetChildByName("FuelListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            foreach (var item in buildingCmp.BuildingTemplate.FuelConsumer.ConsumableFuel)
            {
                MNode fuelNode;

                if (!fuelNodes.ContainsKey(item))
                {
                    fuelNode = CreateFuelNode(item);
                    fuelNodes.Add(item, fuelNode);
                    nodesFuels.Add(fuelNode, item);
                }
                else
                {
                    fuelNode = fuelNodes[item];
                }

                bool allowed = buildingCmp.IsFuelAllowed(item);
                fuelNode.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(allowed);

                listViewScript.AddItem(fuelNode);
            }
        }

        private void UpdateButtons()
        {
            ParentNode.RemoveChild(returnHomeButton);

            ListViewUIScript buttonsListView = ParentNode.GetChildByName("ButtonsListView").GetComponent<ListViewUIScript>();
            buttonsListView.Clear();

            if (selectedBuilding.IsBuilt == false)
            {
                buttonsListView.AddItem(cancelButton);
            }
            else
            {
                if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.FarmPlot)
                {
                    FarmPlot farmPlot = selectedBuilding as FarmPlot;

                    if(farmPlot.IsWild == false)
                    {
                        ProgressTree levelSystem = GameplayScene.Instance.ProgressTree;

                        buttonsListView.AddItem(copySettingsButton);

                        if (levelSystem.IsTechnologyUnlocked(TechnologyDatabase.Irrigation))
                        {
                            irrigateButton.GetComponent<ButtonScript>().IsSelected = farmPlot.Irrigate;
                            buttonsListView.AddItem(irrigateButton);
                        }

                        if (levelSystem.IsTechnologyUnlocked(TechnologyDatabase.Fertilizing))
                        {
                            fertilizeButton.GetComponent<ButtonScript>().IsSelected = farmPlot.Fertilize;
                            buttonsListView.AddItem(fertilizeButton);
                        }
                    }

                    // *** Harvest button
                    if (farmPlot.PlantData.Fruits != null)
                    {
                        cutChopButton.GetComponent<ButtonScript>().IsDisabled = false;

                        if (farmPlot.PlantData.ToolType == ToolType.Harvesting)
                        {
                            cutChopButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.CutIcon;

                            cutChopButton.GetComponent<ButtonScript>().IsSelected = farmPlot.Harvest;

                            cutChopButton.Tooltips = Localization.GetLocalizedText("cut_automatically") + "\n" +
                "/c[#919090]" + Localization.GetLocalizedText("сut_automatically_description") + "/cd";
                        }
                        else if (farmPlot.PlantData.ToolType == ToolType.Woodcutting)
                        {
                            cutChopButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.ChopIcon;

                            if(!GameplayScene.Instance.ProgressTree.IsTechnologyUnlocked(TechnologyDatabase.StoneTools))
                            {
                                cutChopButton.GetComponent<ButtonScript>().IsDisabled = true;
                                cutChopButton.Tooltips = Localization.GetLocalizedText("сhop_automatically") + "\n" +
                "/c[#919090]" + Localization.GetLocalizedText("сhop_automatically_description") + "/cd\n" + 
                $"/c[#FFA500]{Localization.GetLocalizedText("required_technology")}:\n" +
                                    $"{TechnologyDatabase.StoneTools.Name}/cd";
                            }
                            else
                            {
                                cutChopButton.GetComponent<ButtonScript>().IsSelected = farmPlot.Harvest;
                                cutChopButton.Tooltips = Localization.GetLocalizedText("сhop_automatically") + "\n" +
                "/c[#919090]" + Localization.GetLocalizedText("сhop_automatically_description") + "/cd";
                            }
                        }

                        buttonsListView.AddItem(cutChopButton);
                    }

                    // *** Chop button
                    cutChopCompletelyButton.GetComponent<ButtonScript>().IsDisabled = false;

                    if (farmPlot.PlantData.ToolType == ToolType.Harvesting)
                    {
                        cutChopCompletelyButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.CutCompletelyIcon;

                        cutChopCompletelyButton.GetComponent<ButtonScript>().IsSelected = farmPlot.Chop;

                        cutChopCompletelyButton.Tooltips = Localization.GetLocalizedText("cut_now");
                    }

                    buttonsListView.AddItem(cutChopCompletelyButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.TimeMachine)
                {
                    returnHomeButton.X = 8;
                    returnHomeButton.Y = 48;
                    ParentNode.AddChildNode(returnHomeButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Stockpile)
                {
                    if (selectedBuilding.BuildingTemplate.Storage.CanCopySettings)
                    {
                        buttonsListView.AddItem(copySettingsButton);
                    }

                    buttonsListView.AddItem(priorityButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Deposit)
                {
                    DepositCmp depositCmp = selectedBuilding as DepositCmp;

                    if(depositCmp.BuildingTemplate.Deposit.RequiredToolType == ToolType.Pick)
                    {
                        if (!GameplayScene.Instance.ProgressTree.IsTechnologyUnlocked(TechnologyDatabase.StoneTools))
                        {
                            mineButton.GetComponent<ButtonScript>().IsDisabled = true;
                            mineButton.Tooltips = $"{Localization.GetLocalizedText("mine")}\n{Localization.GetLocalizedText("required_technology")}:\n" +
                                    $"{TechnologyDatabase.StoneTools.Name}";
                        }
                        else
                        {
                            mineButton.GetComponent<ButtonScript>().IsDisabled = false;
                            mineButton.Tooltips = $"{Localization.GetLocalizedText("mine")}";
                            mineButton.GetComponent<ButtonScript>().IsSelected = depositCmp.IsMarkedToObtain;
                        }

                        buttonsListView.AddItem(mineButton);
                    }
                    else
                    {
                        gatherButton.GetComponent<ButtonScript>().IsSelected = depositCmp.IsMarkedToObtain;
                        buttonsListView.AddItem(gatherButton);
                    }
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.AnimalPen)
                {
                    buttonsListView.AddItem(copySettingsButton);

                    AnimalPenBuildingCmp animalPen = selectedBuilding as AnimalPenBuildingCmp;

                    cleanUpManureButton.ButtonScript.IsSelected = animalPen.IsFlaggedToCleanManure;

                    buttonsListView.AddItem(cleanUpManureButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Crafter)
                {
                    buttonsListView.AddItem(copySettingsButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Mine)
                {
                    MineBuildingCmp mineBuilding = selectedBuilding as MineBuildingCmp;

                    autoMineSpawnedDepositsButton.GetComponent<ButtonScript>().IsSelected = mineBuilding.AutoMineSpawnedDeposits;
                    buttonsListView.AddItem(autoMineSpawnedDepositsButton);
                }
                else if(selectedBuilding.BuildingTemplate.BuildingType == BuildingType.TradingPost)
                {
                    buttonsListView.AddItem(tradeButton);
                }
            }

            if (selectedBuilding.BuildingTemplate.Cloneable &&
                GameplayScene.Instance.ProgressTree.IsBuildingUnlocked(selectedBuilding.BuildingTemplate))
            {
                cloneButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = selectedBuilding.BuildingTemplate.Icons[Direction.DOWN];
                buttonsListView.AddItem(cloneButton);
            }
        }

        private MNode CreateFuelNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);
            element.Tooltips = item.GetInformation();

            MToggleUI toggle = new MToggleUI(ParentNode.Scene, false);
            toggle.Name = "Toggle";
            toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(OnFuelNodeTogglePressedCallback);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;
            itemIcon.X = toggle.Width + 5;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name}";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 5;

            element.Width = toggle.Width + 5 + itemIcon.Width + 5 + itemName.Width;
            element.Height = 32;

            element.AddChildNode(toggle);
            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            return element;
        }

        private void OnFuelNodeTogglePressedCallback(bool value, MToggleUI toggle)
        {
            Item fuel = nodesFuels[toggle.ParentNode];
            selectedBuilding.SetFuelFilter(fuel, value);
        }


        private void SetTab(bool value, ButtonScript buttonScript)
        {
            foreach (var kvp in tabNodes)
            {
                Tab tab = kvp.Key;
                MNode node = kvp.Value;

                if (tab != buttonScript.ParentNode)
                {
                    tab.Y = -tab.Height;
                    node.Active = false;
                }
                else
                {
                    tab.Y = -tab.Height + 6;
                    node.Active = true;
                }
            }
        }

        private void Interactable_InteractionButton_Pressed(ButtonScript buttonScript)
        {
            InteractionData interactionData = buttonScript.ParentNode.GetMetadata<InteractionData>("interaction_data");
            Interactable interactable = buttonScript.ParentNode.GetMetadata<Interactable>("interactable");

            if (buttonScript.IsSelected)
            {
                selectedInteractable.MarkInteraction(interactionData.InteractionType);
            }
            else
            {
                selectedInteractable.UnmarkInteraction(interactionData.InteractionType);
            }

            buttonScript.ParentNode.Tooltips = GenerateInteractionTooltip(interactable, interactionData);
        }

        private string GenerateInteractionTooltip(Interactable interactable, InteractionData interactionData)
        {
            if (interactable == null) return "";

            if (interactionData == null) return "";

            InteractionType interactionType = interactionData.InteractionType;

            if (interactable.AvailableInteractions.Contains(interactionType) == false) return "";

            string tooltip = "";

            if(interactable.IsInteractionMarked(interactionType))
            {
                tooltip += Localization.GetLocalizedText("cancel_x", interactionData.DisplayName);
            }
            else
            {
                tooltip += interactionData.DisplayName;
            }

            tooltip += $"\n{Localization.GetLocalizedText("labor_type")}: {Labor.GetLaborString(interactionData.LaborType)}";

            return tooltip;
        }
    }
}
