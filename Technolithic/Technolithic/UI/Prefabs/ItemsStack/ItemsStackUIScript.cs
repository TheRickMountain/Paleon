using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ItemsStackUIScript : MScript
    {
        private Dictionary<Item, MNode> itemsNodes;
        private Dictionary<MNode, Item> nodesItemContainers;

        private Tile selectedTile;

        public ItemsStackUIScript() : base(true)
        {
            itemsNodes = new Dictionary<Item, MNode>();
            nodesItemContainers = new Dictionary<MNode, Item>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
          
        }

        public void SetItems(Tile tile)
        {
            selectedTile = tile;

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            // Устанавливаем новые количества для предметов
            foreach(var kvp in tile.Inventory.ItemItemContainerPair)
            {
                Item item = kvp.Key;
                int factWeight = tile.Inventory.GetInventoryFactWeight(item);
                if (factWeight > 0)
                {
                    // Создаем ноду, если предмет не был на складе
                    if (!itemsNodes.ContainsKey(item))
                    {
                        MNode itemNode = CreateItemNode(item);
                        itemsNodes.Add(item, itemNode);
                        nodesItemContainers.Add(itemNode, item);
                    }

                    listViewScript.AddItem(itemsNodes[item]);

                    ((MyText)itemsNodes[item].GetChildByName("Name")).Text = $"{item.Name} [{factWeight}]";
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

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);

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
            element.AddOnIntersectsCallback(ShowPopUp);

            return element;
        }
    
        private void ShowPopUp(MNode node)
        {
            Item item = nodesItemContainers[node];

            if (item == null)
                return;

            string info = item.GetInformation();

            info += selectedTile.Inventory.GetInformation(item);

            GlobalUI.ShowTooltips(info);
        }
    }
}
