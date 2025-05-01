using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EquipmentListViewUI : ListViewUI
    {
        private Dictionary<Item, MNode> itemsNodes = new();

        private CreatureEquipment creatureEquipment;
        private CreatureCmp creature;

        public EquipmentListViewUI(Scene scene, int elementWidth, int elementHeight, int rowsCount, int columnsCount = 1,
            bool scrollable = true, bool showBackground = true, bool scrollerLeft = false) :
            base(scene, elementWidth, elementHeight, rowsCount, columnsCount, scrollable, showBackground, scrollerLeft)
        {
        }

        public void SetEquipment(CreatureCmp creature, CreatureEquipment creatureEquipment)
        {
            this.creature = creature;
            this.creatureEquipment = creatureEquipment;

            UpdateView();   
        }

        private void UpdateView()
        {
            ListViewUIScript listView = GetComponent<ListViewUIScript>();
            listView.Clear();

            foreach (ItemContainer toolItemContainer in creatureEquipment.GetTools())
            {
                AddItemContainer(listView, toolItemContainer);
            }

            AddItemContainer(listView, creatureEquipment.ClothingItemContainer);
            AddItemContainer(listView, creatureEquipment.TopClothingItemContainer);
        }

        private void AddItemContainer(ListViewUIScript listView, ItemContainer itemContainer)
        {
            if (itemContainer == null)
                return;

            Item item = itemContainer.Item;
            int factWeight = itemContainer.FactWeight;

            if (!itemsNodes.ContainsKey(item))
            {
                MNode itemNode = CreateItemNode(item);
                itemsNodes.Add(item, itemNode);
            }

            MNode iconNode = itemsNodes[item].GetChildByName("Icon");

            iconNode.Tooltips = item.GetInformation();

            float durability = itemContainer.Durability;
            iconNode.Tooltips += $"\n{Localization.GetLocalizedText("durability")}: {(int)durability}/{item.Durability}";

            listView.AddItem(itemsNodes[item]);

            ((MyText)itemsNodes[item].GetChildByName("Name")).Text = $"{item.Name} [{factWeight}]";
        }

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);
            
            MButtonUI removeEquipmentButton = new MButtonUI(ParentNode.Scene);
            removeEquipmentButton.Image.Texture = ResourceManager.RemoveEquipmentIcon;
            removeEquipmentButton.Width = 32;
            removeEquipmentButton.Height = 32;
            removeEquipmentButton.Tooltips = Localization.GetLocalizedText("remove_equipment");
            removeEquipmentButton.ButtonScript.AddOnClickedCallback(OnRemoveEquipmentButtonPressed);
            removeEquipmentButton.SetMetadata("equipment", item);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.X = removeEquipmentButton.Width + 5;
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} [0]";
            itemName.Name = "Name";
            itemName.X = itemIcon.X + itemIcon.Width + 5;
            itemName.Width = 100;
            itemName.Height = 32;
            
            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(removeEquipmentButton);
            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            return element;
        }

        private void OnRemoveEquipmentButtonPressed(bool value, ButtonScript buttonScript)
        {
            Item item = buttonScript.ParentNode.GetMetadata<Item>("equipment");

            Tile tile = creature.Movement.CurrentTile;

            if (item.Tool != null)
            {
                ItemContainer toolItemContainer = creature.CreatureEquipment.ToolItemContainer;

                if (toolItemContainer != null)
                {
                    // Попытка снять инструмент, который используется на данный момент
                    if (toolItemContainer.Item.Tool.ToolType == item.Tool.ToolType)
                    {
                        creature.CancelLabor();
                    }
                }

                creatureEquipment.ThrowTool(item.Tool.ToolType, tile);
            }
            else if (item.Outfit != null)
            {
                if (item.Outfit.IsTop)
                {
                    creatureEquipment.ThrowTopClothing(tile);
                }
                else
                {
                    creatureEquipment.ThrowClothing(tile);
                }
            }

            UpdateView();
        }
    }
}
