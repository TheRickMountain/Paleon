using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class StorageUIScript : MScript
    {
        private Dictionary<Item, MNode> itemsNodes;
        private Dictionary<MNode, Item> nodesItems;

        private Dictionary<int, MNode> categoriesNodes;
        private Dictionary<MNode, int> nodesCategories;

        private StorageBuildingCmp selectedStorage;

        private MyText spaceTextCmp;
        private MSliderBar sliderBar;

        private MNode allowBanAllNode;

        private Dictionary<int, bool> categoriesStates = new Dictionary<int, bool>();

        public StorageUIScript() : base(true)
        {
            itemsNodes = new Dictionary<Item, MNode>();
            nodesItems = new Dictionary<MNode, Item>();

            categoriesNodes = new Dictionary<int, MNode>();
            nodesCategories = new Dictionary<MNode, int>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            spaceTextCmp = (MyText)ParentNode.GetChildByName("SpaceInfo");

            sliderBar = (MSliderBar)ParentNode.GetChildByName("SliderBar");
            sliderBar.CurrentValueChanged += OnSliderBarCurrentValueChanged;
        }

        private void OnSliderBarCurrentValueChanged(int value, MSliderBar sliderBar)
        {
            selectedStorage.CurrentCapacity = value;
            spaceTextCmp.Text = $"{Localization.GetLocalizedText("stored")}: {selectedStorage.Inventory.TotalItemsCount} / {selectedStorage.CurrentCapacity}";
        }

        public void SetStockpile(StorageBuildingCmp storage)
        {
            sliderBar.MaxValue = storage.MaxCapacity;
            sliderBar.SetCurrentValueSilent(storage.CurrentCapacity);

            selectedStorage = storage;

            spaceTextCmp.Text = $"{Localization.GetLocalizedText("stored")}: {storage.Inventory.TotalItemsCount} / {storage.CurrentCapacity}";

            storage.OnCapacityChangedCallback += UpdateStorageCapacityInfo;

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();

            listViewScript.Clear();

            if (allowBanAllNode == null)
                allowBanAllNode = CreateAllowBanAllNode();

            listViewScript.AddItem(allowBanAllNode);

            // Отмечаем чекбоксы предметов
            foreach (var kvp in storage.GetSortedFilters())
            {
                Item item = kvp.Key;
                bool allow = kvp.Value;
                int count = storage.Inventory.GetInventoryFactWeight(item);

                if(!categoriesNodes.ContainsKey(item.ItemCategory))
                {
                    MNode categoryNode = CreateCategoryNode(item.ItemCategory);
                    categoriesNodes.Add(item.ItemCategory, categoryNode);
                    nodesCategories.Add(categoryNode, item.ItemCategory);

                    categoriesStates.Add(item.ItemCategory, true);
                }

                if (!listViewScript.ContainsItem(categoriesNodes[item.ItemCategory]))
                    listViewScript.AddItem(categoriesNodes[item.ItemCategory]);

                // Создаем ноду, если предмет не был на складе
                if (!itemsNodes.ContainsKey(item))
                {
                    MNode listElementNode = CreateListElementNode(item);
                    itemsNodes.Add(item, listElementNode);
                    nodesItems.Add(listElementNode, item);
                }

                MNode node = itemsNodes[item];

                if(IsCategoryExpanded(item.ItemCategory))
                {
                    listViewScript.AddItem(node);
                }

                Color nodeColor = count > 0 ? Color.White : Color.White * 0.5f;

                MyText textCmp = ((MyText)node.GetChildByName("Name"));
                textCmp.Text = $"{item.Name} [{count}]";
                textCmp.Color = nodeColor;

                MImageCmp iconCmp = node.GetChildByName("Icon").GetComponent<MImageCmp>();
                iconCmp.Color = nodeColor;

                ToggleScript toggleScript = node.GetChildByName("Toggle").GetComponent<ToggleScript>();
                toggleScript.SilentCheck(allow);
            }
        }

        private void UpdateItemsCheckboxes()
        {
            foreach(var kvp in itemsNodes)
            {
                Item item = kvp.Key;
                MNode node = kvp.Value;

                ToggleScript toggleScript = node.GetChildByName("Toggle").GetComponent<ToggleScript>();

                if (selectedStorage.IsItemAllowed(item))
                {
                    toggleScript.SilentCheck(true);
                }
                else
                {
                    toggleScript.SilentCheck(false);
                }
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private void UpdateStorageCapacityInfo(StorageBuildingCmp storage)
        {
            spaceTextCmp.Text = $"{Localization.GetLocalizedText("stored")}: {selectedStorage.Inventory.TotalItemsCount} / {selectedStorage.CurrentCapacity}";
        }

        private MNode CreateListElementNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);

            MToggleUI toggleUI = new MToggleUI(ParentNode.Scene, true);
            toggleUI.Name = "Toggle";
            toggleUI.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetToggle);
            toggleUI.X = 8;

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.X = toggleUI.LocalX + toggleUI.Width + 5;
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} [0]";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 5;

            element.Width = toggleUI.LocalX + itemName.LocalX + itemName.Width;
            element.Height = itemIcon.Height;

            element.AddChildNode(toggleUI);
            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);
            element.AddOnIntersectsCallback(ShowPopUp);

            return element;
        }

        private MNode CreateCategoryNode(int itemCategory)
        {
            MNode element = new MNode(ParentNode.Scene);

            SmallButton expandButton = new SmallButton(ParentNode.Scene, null);
            expandButton.GetComponent<ButtonScript>().BackgroundImage.Texture = ResourceManager.GetTexture("ui").GetSubtexture(64, 48, 16, 16);
            expandButton.GetComponent<ButtonScript>().AddOnClickedCallback(ExpandCategory);
            expandButton.SetMetadata("itemCategory", itemCategory);
            
            SmallButton allowButton = new SmallButton(ParentNode.Scene, ResourceManager.GetTexture("ui").GetSubtexture(128, 128, 16, 16));
            allowButton.SetMetadata("itemCategory", itemCategory);
            allowButton.GetComponent<ButtonScript>().AddOnClickedCallback(AllowCategory);
            allowButton.X = expandButton.LocalX + expandButton.Width + 5;

            SmallButton clearButton = new SmallButton(ParentNode.Scene, ResourceManager.GetTexture("ui").GetSubtexture(144, 128, 16, 16));
            clearButton.GetComponent<ButtonScript>().AddOnClickedCallback(ClearCategory);
            clearButton.SetMetadata("itemCategory", itemCategory);
            clearButton.X = allowButton.LocalX + allowButton.Width + 5;

            string categoryName = ItemDatabase.ItemCategories[itemCategory];

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = categoryName;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = clearButton.LocalX + clearButton.Width + 5;
            itemName.Color = Color.LightBlue;

            element.Width = allowButton.LocalX + itemName.LocalX + itemName.Width;
            element.Height = allowButton.Height;

            element.AddChildNode(expandButton);
            element.AddChildNode(allowButton);
            element.AddChildNode(clearButton);
            element.AddChildNode(itemName);

            return element;
        }

        private void OnAllowAllButtonPressed(bool value, ButtonScript buttonScript)
        {
            selectedStorage.SetAllowanceMode(AllowanceMode.All);

            UpdateItemsCheckboxes();
        }

        private void OnBanAllButtonPressed(bool value, ButtonScript buttonScript)
        {
            selectedStorage.SetAllowanceMode(AllowanceMode.Nothing);

            UpdateItemsCheckboxes();
        }

        private void AllowCategory(bool value, ButtonScript buttonScript)
        {
            int itemCategory = buttonScript.ParentNode.GetMetadata<int>("itemCategory");

            selectedStorage.AllowCategory(itemCategory);

            UpdateItemsCheckboxes();
        }

        private void ClearCategory(bool value, ButtonScript buttonScript)
        {
            int itemCategory = buttonScript.ParentNode.GetMetadata<int>("itemCategory");

            selectedStorage.ClearCategory(itemCategory);

            UpdateItemsCheckboxes();
        }

        private void ExpandCategory(bool value, ButtonScript buttonScript)
        {
            int itemCategory = buttonScript.ParentNode.GetMetadata<int>("itemCategory");

            bool isExpanded = IsCategoryExpanded(itemCategory);

            isExpanded = !isExpanded;

            categoriesStates[itemCategory] = isExpanded;

            if (isExpanded)
            {
                buttonScript.BackgroundImage.Texture = ResourceManager.GetTexture("ui").GetSubtexture(64, 48, 16, 16);
            }
            else
            {
                buttonScript.BackgroundImage.Texture = ResourceManager.GetTexture("ui").GetSubtexture(80, 80, 16, 16);
            }

            

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();

            for (int i = ItemDatabase.Items.Count - 1; i >= 0; i--)
            {
                Item item = ItemDatabase.Items.ElementAt(i).Value;
                if (item.ItemCategory == itemCategory)
                {
                    if (isExpanded)
                    {
                        listViewScript.AddItemAfter(itemsNodes[item], buttonScript.ParentNode.ParentNode);
                    }
                    else
                    {
                        listViewScript.RemoveItem(itemsNodes[item]);
                    }
                }
            }

            listViewScript.UpdateView(true);
        }

        private MNode CreateAllowBanAllNode()
        {
            MNode element = new MNode(ParentNode.Scene);

            SmallButton allowButton = new SmallButton(ParentNode.Scene, ResourceManager.GetTexture("ui").GetSubtexture(128, 128, 16, 16));
            allowButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnAllowAllButtonPressed);
            allowButton.X = 5;

            SmallButton clearButton = new SmallButton(ParentNode.Scene, ResourceManager.GetTexture("ui").GetSubtexture(144, 128, 16, 16));
            clearButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnBanAllButtonPressed);
            clearButton.X = allowButton.LocalX + allowButton.Width + 5;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = Localization.GetLocalizedText("all");
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = clearButton.LocalX + clearButton.Width + 5;
            itemName.Color = Color.LightBlue;

            element.Width = allowButton.LocalX + itemName.LocalX + itemName.Width;
            element.Height = allowButton.Height;

            element.AddChildNode(allowButton);
            element.AddChildNode(clearButton);
            element.AddChildNode(itemName);

            return element;
        }

        private void ShowPopUp(MNode node)
        {
            if (nodesItems.ContainsKey(node))
            {
                Item item = nodesItems[node];

                string info = item.GetInformation();

                info += selectedStorage.Inventory.GetInformation(item);

                GlobalUI.ShowTooltips(info);
            }
        }

        private void SetToggle(bool value, MToggleUI toggleUI)
        {
            if (nodesItems.ContainsKey(toggleUI.ParentNode))
            {
                Item item = nodesItems[toggleUI.ParentNode];

                selectedStorage.SetItemFilter(item, toggleUI.GetComponent<ToggleScript>().IsOn);
            }
            else if(nodesCategories.ContainsKey(toggleUI.ParentNode))
            {
                int itemCategory = nodesCategories[toggleUI.ParentNode];

                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;

                    if (item.ItemCategory == itemCategory)
                    {
                        bool isOn = toggleUI.GetComponent<ToggleScript>().IsOn;
                        selectedStorage.SetItemFilter(item, isOn);
                        if (itemsNodes.ContainsKey(item))
                        {
                            itemsNodes[item].GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(isOn);
                        }
                    }
                }
            }
        }

        private bool IsCategoryExpanded(int itemCategory)
        {
            if(categoriesStates.ContainsKey(itemCategory) == false)
            {
                return false;
            }

            return categoriesStates[itemCategory];
        }
    }
}
