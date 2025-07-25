using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingUIScript : InteractableUIScript
    {
        private Dictionary<Item, MNode> fuelNodes;
        private Dictionary<MNode, Item> nodesFuels;

        private BuildingCmp selectedBuilding;
        private MyPanelUI panel;

        private BigButton cloneButton;
        private BigButton copySettingsButton; // TODO: эту кнопку нужно перенести в дополнительные окна, где непосредственно расположены все настройки
        private BigButton priorityButton; 
        private BigButton autoMineSpawnedDepositsButton;
        private BigButton tradeButton;
        private MTextButtonUI returnHomeButton;

        private Dictionary<Tab, MNode> tabNodes = new Dictionary<Tab, MNode>();

        private RichTextUI infoText;

        public BuildingUIScript()
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

            cloneButton = new BigButton(ParentNode.Scene, RenderManager.Pixel, false);
            cloneButton.Tooltips = Localization.GetLocalizedText("clone") + " [Q]";
            cloneButton.GetComponent<ButtonScript>().AddOnClickedCallback(CloneBuilding);

            copySettingsButton = new BigButton(ParentNode.Scene, ResourceManager.CopyIcon, false);
            copySettingsButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnCopySettingsButtonPressedCallback);
            copySettingsButton.Tooltips = Localization.GetLocalizedText("copy_settings");

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
            if (panel.Intersects(mouseX, mouseY))
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

        public void SetBuilding(BuildingCmp building, ProgressTree progressTree)
        {
            selectedBuilding = building;

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

            SetInteractable(building);
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

        public void CloneBuilding(bool value, ButtonScript sender)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.Build, null);
            GameplayScene.WorldManager.SetBuilding(selectedBuilding.BuildingTemplate);
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

            if (selectedBuilding.IsBuilt)
            {
                if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.FarmPlot)
                {
                    FarmPlot farmPlot = selectedBuilding as FarmPlot;

                    if (farmPlot.IsWild == false)
                    {
                        buttonsListView.AddItem(copySettingsButton);
                    }
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.TimeMachine)
                {
                    returnHomeButton.X = 8;
                    returnHomeButton.Y = 48;
                    ParentNode.AddChildNode(returnHomeButton);
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Stockpile)
                {
                    if (selectedBuilding.BuildingTemplate.Storage.CanCopySettings)
                    {
                        buttonsListView.AddItem(copySettingsButton);
                    }

                    buttonsListView.AddItem(priorityButton);
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.AnimalPen)
                {
                    buttonsListView.AddItem(copySettingsButton);
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Crafter)
                {
                    buttonsListView.AddItem(copySettingsButton);
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.Mine)
                {
                    MineBuildingCmp mineBuilding = selectedBuilding as MineBuildingCmp;

                    autoMineSpawnedDepositsButton.GetComponent<ButtonScript>().IsSelected = mineBuilding.AutoMineSpawnedDeposits;
                    buttonsListView.AddItem(autoMineSpawnedDepositsButton);
                }
                else if (selectedBuilding.BuildingTemplate.BuildingType == BuildingType.TradingPost)
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
    }
}
