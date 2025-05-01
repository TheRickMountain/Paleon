using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ResourcesUIScript : MScript
    {

        private SmallButton closeButton;

        private Dictionary<Item, MNode> itemNodePairs = new Dictionary<Item, MNode>();
        private Dictionary<MNode, Item> nodeItemPairs = new Dictionary<MNode, Item>();

        private ListViewUIScript listViewScript;

        public ResourcesUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            // Кнопка закрытия окна
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);
            ParentNode.AddChildNode(closeButton);

            MNode listView = ParentNode.GetChildByName("ListView");
            listViewScript = listView.GetComponent<ListViewUIScript>();
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);

            MToggleUI toggleUI = new MToggleUI(ParentNode.Scene, true);
            toggleUI.Name = "Toggle";
            toggleUI.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetToggle);
            toggleUI.X = 8;
            toggleUI.Tooltips = Localization.GetLocalizedText("show_resource_in_the_general_list");

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.X = toggleUI.LocalX + toggleUI.Width + 5;
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name}";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 5;

            MyText itemAmount = new MyText(ParentNode.Scene);
            itemAmount.Text = $"{10000}";
            itemAmount.Name = "Amount";
            itemAmount.Width = 48;
            itemAmount.Height = 32;
            itemAmount.X = ParentNode.Width - (16 + 16 + itemAmount.Width);

            element.AddChildNode(toggleUI);
            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);
            element.AddChildNode(itemAmount);

            element.Width = ParentNode.Width - (16 + 16); // borders + scroller width
            element.Height = 34;

            return element;
        }

        public void Open()
        {
            listViewScript.Clear();

            foreach (var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                if(GameplayScene.WorldManager.IsItemOpened(item) == false)
                {
                    continue;
                }

                MNode element;

                if (!itemNodePairs.ContainsKey(item))
                {
                    element = CreateItemNode(item);

                    itemNodePairs.Add(item, element);

                    nodeItemPairs.Add(element, item);
                }
                else
                {
                    element = itemNodePairs[item];
                }

                listViewScript.AddItem(element);

                int itemAmount = GameplayScene.Instance.TotalResourcesChart.GetItemCount(item);

                UpdateItemAmountText(item, itemAmount);

                if(GameplayScene.WorldManager.IsItemVisible(item) == false)
                {
                    element.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(false);
                }
            }
        }

        public void UpdateItemAmountText(Item item, int amount)
        {
            ((MyText)itemNodePairs[item].GetChildByName("Amount")).Text = $"{amount}";
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

        private void SetToggle(bool value, MToggleUI toggleUI)
        {
            Item item = nodeItemPairs[toggleUI.ParentNode];

            if(value)
            {
                GameplayScene.WorldManager.ShowItem(item);
            }
            else
            {
                GameplayScene.WorldManager.HideItem(item);
            }
        }
    }
}
