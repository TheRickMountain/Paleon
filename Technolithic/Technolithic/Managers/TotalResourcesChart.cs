using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class TotalResourcesChart
    {

        private Dictionary<Item, int> itemsCount;

        public Action<Item, int> CbOnItemCountChanged { get; set; }

        public int TotalSettlersFoodCount { get; private set; } = 0;

        public TotalResourcesChart()
        {
            itemsCount = new Dictionary<Item, int>();
        }

        public void AddItem(Item item, int weight)
        {
            if (itemsCount.ContainsKey(item) == false)
                itemsCount.Add(item, weight);
            else
                itemsCount[item] += weight;

            if (item.Consumable != null && item.Consumable.GetValueOf(AttributeType.Satiety) > 0
                && Engine.Instance.SettlerRation.Contains(item))
            {
                TotalSettlersFoodCount += weight;
            }

            foreach (var kvp in GameplayScene.WorldManager.CrafterBuildings)
            {
                foreach (var crafter in kvp.Value)
                {
                    crafter.UpdateTargetCraftingRecipe();
                }
            }

            foreach (var kvp in GameplayScene.WorldManager.AutoCrafterBuildings)
            {
                foreach (var crafter in kvp.Value)
                {
                    crafter.UpdateTargetCraftingRecipe();
                }
            }

            CbOnItemCountChanged?.Invoke(item, GetItemCount(item));
        }

        public int GetItemCount(Item item)
        {
            if (itemsCount.ContainsKey(item) == false)
                return 0;

            return itemsCount[item];
        }

    }
}
