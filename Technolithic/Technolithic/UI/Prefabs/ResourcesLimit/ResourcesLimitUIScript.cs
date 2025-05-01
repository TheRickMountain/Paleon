using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ResourcesLimitUIScript : MScript
    {

        private SmallButton closeButton;

        private MButtonUI addButton;
        private MButtonUI subButton;
        private MButtonUI produceEndleslyButton;

        private Item firstSelectedItem;
        private Item lastSelectedItem;

        private List<Item> selectedItems;

        private Dictionary<Item, MNode> itemNodePairs = new Dictionary<Item, MNode>();
        private Dictionary<MNode, Item> nodeItemPairs = new Dictionary<MNode, Item>();

        private ResourcesLimitManager resourcesLimitManager;

        private ListViewUIScript listViewScript;

        public ResourcesLimitUIScript() : base(true)
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

            addButton = (MButtonUI)ParentNode.GetChildByName("AddButton");
            subButton = (MButtonUI)ParentNode.GetChildByName("SubButton");
            produceEndleslyButton = (MButtonUI)ParentNode.GetChildByName("ProduceEndleslyButton");

            addButton.GetComponent<ButtonScript>().AddOnClickedCallback(Add);
            addButton.Tooltips = "Left Click - Add 1\nShift + Left Click - Add 5\nCtrl + Left Click - Add 10";

            subButton.GetComponent<ButtonScript>().AddOnClickedCallback(Sub);
            subButton.Tooltips = "Left Click - Remove 1\nShift + Left Click - Remove 5\nCtrl + Left Click - Remove 10";

            produceEndleslyButton.GetComponent<ButtonScript>().AddOnClickedCallback(Repeat);
            produceEndleslyButton.Tooltips = Localization.GetLocalizedText("repeat");

            selectedItems = new List<Item>();

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
            MButtonUI element = new MButtonUI(ParentNode.Scene);
            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.BackgroundOverlap = 2;
            element.Image.SetBorder(8, 8, 8, 8);
            element.ButtonScript.AddOnClickedCallback(SelectItemToLimit);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.X = 8;
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} [0]";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 5;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            element.Width = ParentNode.Width - (16 + 16); // borders + scroller width
            element.Height = 34;

            return element;
        }

        public void SelectItemToLimit(bool value, ButtonScript sender)
        {
            foreach (var kvp in itemNodePairs)
            {
                itemNodePairs[kvp.Key].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            }

            selectedItems.Clear();

            if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                lastSelectedItem = nodeItemPairs[(MButtonUI)sender.ParentNode];

                if (firstSelectedItem == null)
                {
                    firstSelectedItem = lastSelectedItem;
                }
            }
            else
            {
                firstSelectedItem = nodeItemPairs[(MButtonUI)sender.ParentNode];
                lastSelectedItem = firstSelectedItem;
            }

            foreach (var itemNode in listViewScript.GetItemsBetween(itemNodePairs[firstSelectedItem], itemNodePairs[lastSelectedItem]))
            {
                Item item = nodeItemPairs[itemNode];

                selectedItems.Add(item);

                itemNode.GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);
            }
        }

        public void Open(ResourcesLimitManager resourcesLimitManager)
        {
            this.resourcesLimitManager = resourcesLimitManager;

            foreach (var kvp in itemNodePairs)
            {
                itemNodePairs[kvp.Key].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            }

            selectedItems.Clear();

            firstSelectedItem = null;
            lastSelectedItem = null;

            listViewScript.Clear();

            foreach (var item in resourcesLimitManager.GetAvailableItems())
            {
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

                int itemLimit = resourcesLimitManager.GetItemLimit(item);

                UpdateItemLimitText(item, itemLimit);
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

        public void UpdateItemLimitText(Item item, int limit)
        {
            string text = "" + limit;

            if(limit == -1)
            {
                text = "R";
            }

            ((MyText)itemNodePairs[item].GetChildByName("Name")).Text = $"{item.Name} [{text}]";
        }

        private void Add(bool value, ButtonScript sender)
        {
            foreach (var item in selectedItems)
            {
                int itemLimit = resourcesLimitManager.GetItemLimit(item);

                if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Min(99, itemLimit + 5));
                }
                else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Min(99, itemLimit + 10));
                }
                else
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Min(99, itemLimit + 1));
                }

                itemLimit = resourcesLimitManager.GetItemLimit(item);

                UpdateItemLimitText(item, itemLimit);
            }
        }

        private void Sub(bool value, ButtonScript sender)
        {
            foreach (var item in selectedItems)
            {
                int itemLimit = resourcesLimitManager.GetItemLimit(item);

                if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Max(0, itemLimit - 5));
                }
                else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Max(0, itemLimit - 10));
                }
                else
                {
                    resourcesLimitManager.SetItemLimit(item, Math.Max(0, itemLimit - 1));
                }

                itemLimit = resourcesLimitManager.GetItemLimit(item);

                UpdateItemLimitText(item, itemLimit);
            }
        }

        private void Repeat(bool value, ButtonScript sender)
        {
            foreach (var item in selectedItems)
            {
                int itemLimit = resourcesLimitManager.GetItemLimit(item);

                if (itemLimit == -1)
                {
                    resourcesLimitManager.SetItemLimit(item, 0);
                }
                else
                {
                    resourcesLimitManager.SetItemLimit(item, -1);
                }

                itemLimit = resourcesLimitManager.GetItemLimit(item);

                UpdateItemLimitText(item, itemLimit);
            }
        }
    }
}
