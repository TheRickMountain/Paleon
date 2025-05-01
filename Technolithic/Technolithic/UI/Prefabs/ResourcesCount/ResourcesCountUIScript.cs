using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Technolithic
{
    public class ResourcesCountUIScript : MScript
    {
        private Dictionary<Item, MNode> itemsNodes = new Dictionary<Item, MNode>();

        private ListViewUIScript listViewScript;

        public ResourcesCountUIScript() : base(false)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            listViewScript = ParentNode.GetComponent<ListViewUIScript>();

            GameplayScene.Instance.TotalResourcesChart.CbOnItemCountChanged += OnItemCountChangedCallback;
            GameplayScene.WorldManager.ItemVisibilityChanged += OnItemVisibilityChangedCallback;

            UpdateData();
        }

        private void UpdateData()
        {
            foreach(var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;
                if(GameplayScene.WorldManager.IsItemOpened(item) && GameplayScene.WorldManager.IsItemVisible(item))
                {
                    int itemCount = GameplayScene.Instance.TotalResourcesChart.GetItemCount(item);
                    OnItemCountChangedCallback(item, itemCount);
                }
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            
        }

        private void OnItemCountChangedCallback(Item item, int count)
        {
            if(itemsNodes.ContainsKey(item) == false)
            {
                MNode node = CreateItemNode(item);
                itemsNodes.Add(item, node);

                if (GameplayScene.WorldManager.IsItemVisible(item))
                {
                    listViewScript.AddItem(node);
                }
            }

            UpdateItemData(item, count);
        }

        private void OnItemVisibilityChangedCallback(Item item, bool visible)
        {
            MNode node;

            if(itemsNodes.ContainsKey(item))
            {
                node = itemsNodes[item];
            }   
            else
            {
                node = CreateItemNode(item);
                itemsNodes.Add(item, node);
            }

            if(visible)
            {
                listViewScript.AddItem(node);

                int itemCount = GameplayScene.Instance.TotalResourcesChart.GetItemCount(item);
                UpdateItemData(item, itemCount);
            }
            else
            {
                listViewScript.RemoveItem(node);
            }
        }

        private void UpdateItemData(Item item, int count)
        {
            ((MyText)itemsNodes[item].GetChildByName("Text")).Text = $"{count}";
        }

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;
            itemIcon.X = 8;
            itemIcon.Y = 8;

            MyText itemLimitText = new MyText(ParentNode.Scene);
            itemLimitText.Text = "";
            itemLimitText.Name = "Text";
            itemLimitText.Width = 50;
            itemLimitText.Height = 32;
            itemLimitText.X = itemIcon.LocalX + itemIcon.Width + 5;
            itemLimitText.Y = 2;
            itemLimitText.Outlined = true;

            element.Width = itemIcon.Width + 5 + itemLimitText.Width + 16; // borders + scroller width
            element.Height = itemIcon.Height + 16;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemLimitText);

            return element;
        }

    }
}
