using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class InventoryListViewUI : ListViewUI
    {
        private Dictionary<Item, MNode> itemsNodes = new();

        public InventoryListViewUI(Scene scene, int elementWidth, int elementHeight, int rowsCount, int columnsCount = 1, 
            bool scrollable = true, bool showBackground = true, bool scrollerLeft = false) : 
            base(scene, elementWidth, elementHeight, rowsCount, columnsCount, scrollable, showBackground, scrollerLeft)
        {
        }

        public void SetInventory(Inventory inventory)
        {
            ListViewUIScript listView = GetComponent<ListViewUIScript>();
            listView.Clear();

            // Устанавливаем новые количества для предметов
            foreach (var kvp in inventory.ItemItemContainerPair)
            {
                Item item = kvp.Key;
                int factWeight = inventory.GetInventoryFactWeight(item);
                if (factWeight > 0)
                {
                    if (!itemsNodes.ContainsKey(item))
                    {
                        MNode itemNode = CreateItemNode(item);
                        itemsNodes.Add(item, itemNode);
                    }

                    listView.AddItem(itemsNodes[item]);

                    ((MyText)itemsNodes[item].GetChildByName("Name")).Text = $"{item.Name} [{factWeight}]";
                }
            }
        }

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);
            element.Tooltips = item.GetInformation();

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} [0]";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.Width + 5;

            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            return element;
        }
    }
}
