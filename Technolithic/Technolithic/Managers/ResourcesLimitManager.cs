using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ResourcesLimitManager
    {

        private Dictionary<Item, int> resourcesLimits;

        public ResourcesLimitManager(Dictionary<int, int> saveData)
        {
            resourcesLimits = new Dictionary<Item, int>();

            foreach(var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                // -1 - Бесконечно
                resourcesLimits.Add(item, -1);
            }

            if(saveData != null)
            {
                foreach(var kvp in saveData)
                {
                    int itemId = kvp.Key;

                    Item item = ItemDatabase.GetItemById(itemId);

                    if(item == null)
                    {
                        continue;
                    }

                    int limit = kvp.Value;

                    resourcesLimits[item] = limit;
                }
            }
        }

        public int GetItemLimit(Item item)
        {
            if (item == null)
                return 0;

            return resourcesLimits[item];
        }

        public void SetItemLimit(Item item, int limit)
        {
            if (item == null)
                return;

            resourcesLimits[item] = limit;

            foreach(var crafter in GameplayScene.WorldManager.CraftersSortedByMainItems[item])
            {
                crafter.UpdateTargetCraftingRecipe();
            }
        }

        public IEnumerable<Item> GetAvailableItems()
        {
            foreach (var item in GameplayScene.WorldManager.CraftersOpenedItems)
            {
                if (item.IsVirtual)
                    continue;

                if (GameplayScene.Instance.ProgressTree.UnlockedItems[item] == false)
                    continue;

                yield return item;
            }
        }

        public Dictionary<int, int> GetSaveData()
        {
            Dictionary<int, int> saveData = new Dictionary<int, int>();

            foreach(var kvp in resourcesLimits)
            {
                saveData.Add(kvp.Key.Id, kvp.Value);
            }

            return saveData;
        }

    }
}
