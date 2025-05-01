using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ItemsDecayer
    {

        public List<Inventory> Inventories { get; private set; }

        private float timer;

        private Dictionary<Inventory, List<ItemContainer>> itemContainersToRemove;

        public ItemsDecayer()
        {
            Inventories = new List<Inventory>();
            itemContainersToRemove = new Dictionary<Inventory, List<ItemContainer>>();
        }

        public void Update(Season currentSeason)
        {
            timer += Engine.GameDeltaTime;

            if (timer >= 1.0f)
            {
                // Looking for spoiled items
                timer = 0;
                foreach (var inventory in Inventories)
                {
                    for (int i = 0; i < ItemDatabase.Decayable.Count; i++)
                    {
                        Item item = ItemDatabase.Decayable[i];
                        float spoilagePerMinute = item.SpoilageRate / (float)(WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
                        
                        if(currentSeason == Season.Winter)
                        {
                            spoilagePerMinute /= 3f;
                        }
                        else
                        {
                            if (inventory.BuildingCmp != null && inventory.BuildingCmp.BuildingTemplate.BuildingType == BuildingType.Stockpile)
                            {
                                spoilagePerMinute /= inventory.BuildingCmp.BuildingTemplate.Storage.ShelfLifeOfProducts;
                            }
                        }

                        List<ItemContainer> itemContainers;
                        inventory.ItemItemContainerPair.TryGetValue(item, out itemContainers);
                        if(itemContainers != null)
                        {
                            foreach (var itemContainer in itemContainers)
                            {
                                itemContainer.Durability -= spoilagePerMinute;

                                if (itemContainer.Durability <= 0)
                                {
                                    GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                                        .AddNotification($"{item.Name} {Localization.GetLocalizedText("spoiled").ToLower()}", NotificationLevel.WARNING);

                                    if (itemContainersToRemove.ContainsKey(inventory) == false)
                                    {
                                        itemContainersToRemove.Add(inventory, new List<ItemContainer>());
                                    }

                                    itemContainersToRemove[inventory].Add(itemContainer);
                                }
                            }
                        }
                    }
                }
            
                // Removing spoiled items
                foreach(var kvp in itemContainersToRemove)
                {
                    Inventory inventory = kvp.Key;
                    List<ItemContainer> itemContainers = kvp.Value;

                    foreach(var itemContainer in itemContainers)
                    {
                        inventory.PopItemContainer(itemContainer);
                    }
                }

                itemContainersToRemove.Clear();
            }
        }
    }
}
