using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class StorageManager
    {

        public List<StorageBuildingCmp> Storages { get; private set; }

        private Dictionary<Item, int> itemsCount;

        public Action<Item, int> OnItemsCountChangedCallback { get; set; }

        public Dictionary<int, Dictionary<Item, List<Inventory>>> StorageThatHaveEmptySpaceForItems { get; private set; } = new Dictionary<int, Dictionary<Item, List<Inventory>>>();

        public StorageManager()
        {
            Storages = new List<StorageBuildingCmp>();
            itemsCount = new Dictionary<Item, int>();
            StorageThatHaveEmptySpaceForItems.Add(0, new Dictionary<Item, List<Inventory>>());
        }

        public StorageBuildingCmp GetStorageWith(CreatureCmp creatureCmp, Item item)
        {
            if (GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item) == false)
                return null;

            if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Count == 0)
                return null;

            foreach (var inventory in GameplayScene.WorldManager.StoragesThatHaveItemsV2[item])
            {
                if (inventory.GetAvailableItemCount(item) > 0 && inventory.GetReachableTile(creatureCmp) != null)
                    return inventory.BuildingCmp as StorageBuildingCmp;
            }

            return null;
        }

        public StorageBuildingCmp GetStorageWithToolItem(CreatureCmp creature, ToolType toolType)
        {
            if (ItemDatabase.Tools[creature.CreatureType].ContainsKey(toolType) == false)
                return null;

            foreach (var item in ItemDatabase.Tools[creature.CreatureType][toolType])
            {
                StorageBuildingCmp storageBuildingCmp = GetStorageWith(creature, item);
                if (storageBuildingCmp != null)
                    return storageBuildingCmp;
            }

            return null;
        }

        public void AddStorage(StorageBuildingCmp stockpileBuilding)
        {
            Storages.Add(stockpileBuilding);

            stockpileBuilding.OnBuildingCanceledCallback += OnStorageDeletedCallback;
            stockpileBuilding.OnBuildingDestructedCallback += OnStorageDeletedCallback;

            stockpileBuilding.Inventory.OnItemAddedCallback += OnItemAddedCallback;
            stockpileBuilding.Inventory.OnItemRemovedCallback += OnItemRemovedCallback;
            stockpileBuilding.Inventory.OnItemToAddAddedCallback += OnItemToAddAddedCallback;
            stockpileBuilding.OnItemsFilterAllowed += OnStorageItemsFilterAllowedCallback;
            stockpileBuilding.OnItemsFilterForbidden += OnStorageItemsFilterForbiddenCallback;
            stockpileBuilding.OnCapacityChangedCallback += OnStorageCapacityChanged;

            stockpileBuilding.SetAllowanceMode(AllowanceMode.All);
        }

        private void OnStorageDeletedCallback(BuildingCmp building)
        {
            StorageBuildingCmp storageBuildingCmp = building as StorageBuildingCmp;

            Storages.Remove(storageBuildingCmp);
            RemoveStorageFromAllCollections(storageBuildingCmp);

            storageBuildingCmp.OnBuildingCanceledCallback -= OnStorageDeletedCallback;
            storageBuildingCmp.OnBuildingDestructedCallback -= OnStorageDeletedCallback;

            storageBuildingCmp.Inventory.OnItemAddedCallback -= OnItemAddedCallback;
            storageBuildingCmp.Inventory.OnItemToAddAddedCallback -= OnItemToAddAddedCallback;
            storageBuildingCmp.Inventory.OnItemRemovedCallback -= OnItemRemovedCallback;
            storageBuildingCmp.OnItemsFilterAllowed -= OnStorageItemsFilterAllowedCallback;
            storageBuildingCmp.OnItemsFilterForbidden -= OnStorageItemsFilterForbiddenCallback;
            storageBuildingCmp.OnCapacityChangedCallback -= OnStorageCapacityChanged;

        }

        private void OnItemToAddAddedCallback(Inventory senderInventory, Item toAddItem, int toAddWeight)
        {
            StorageBuildingCmp storageBuildingCmp = senderInventory.BuildingCmp as StorageBuildingCmp;
            int roomId = senderInventory.GetRoom().Id;

            if (toAddWeight > 0)
            {
                if (storageBuildingCmp.EmptySpaceCount <= 0)
                {
                    Inventory storageInventory = storageBuildingCmp.Inventory;

                    foreach (var kvp in ItemDatabase.Items)
                    {
                        Item item = kvp.Value;
                        if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item))
                        {
                            if (StorageThatHaveEmptySpaceForItems[roomId][item].Contains(storageInventory))
                            {
                                StorageThatHaveEmptySpaceForItems[roomId][item].Remove(storageInventory);
                            }
                        }
                    }
                }
            }
            else
            {
                if (storageBuildingCmp.EmptySpaceCount == Math.Abs(toAddWeight))
                {
                    foreach (var kvp in ItemDatabase.Items)
                    {
                        Item item = kvp.Value;
                        if (storageBuildingCmp.Storage.Filters.Contains(item.ItemCategory) && storageBuildingCmp.IsItemAllowed(item))
                        {
                            AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, item);
                        }
                    }

                    SortStorageThatHaveEmptySpaceForItemsByPriority();
                }
            }
        }

        private void OnItemAddedCallback(Inventory senderInventory, Item item, int weight)
        {
            if (itemsCount.ContainsKey(item) == false)
                itemsCount.Add(item, 0);

            itemsCount[item] += weight;

            StorageBuildingCmp storageBuildingCmp = senderInventory.BuildingCmp as StorageBuildingCmp;
            int roomId = senderInventory.GetRoom().Id;

            OnItemsCountChangedCallback?.Invoke(item, itemsCount[item]);
        }

        private void OnItemRemovedCallback(Inventory senderInventory, Item removedItem, int count)
        {
            if (count == 0)
                return;

            itemsCount[removedItem] -= count;

            StorageBuildingCmp storageBuildingCmp = (senderInventory.BuildingCmp as StorageBuildingCmp);
            int roomId = senderInventory.GetRoom().Id;

            int emptySpaceCount = storageBuildingCmp.EmptySpaceCount;

            if (emptySpaceCount <= 0)
            {
                Inventory storageInventory = storageBuildingCmp.Inventory;

                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;
                    if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item))
                    {
                        if (StorageThatHaveEmptySpaceForItems[roomId][item].Contains(storageInventory))
                        {
                            StorageThatHaveEmptySpaceForItems[roomId][item].Remove(storageInventory);
                        }
                    }
                }
            }
            else
            {
                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;
                    if (storageBuildingCmp.Storage.Filters.Contains(item.ItemCategory) && storageBuildingCmp.IsItemAllowed(item))
                    {
                        AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, item);
                    }
                }

                SortStorageThatHaveEmptySpaceForItemsByPriority();
            }

            OnItemsCountChangedCallback?.Invoke(removedItem, itemsCount[removedItem]);
        }

        private void OnStorageItemsFilterAllowedCallback(StorageBuildingCmp storageBuildingCmp, Item allowedItem)
        {
            int emptySpaceCount = storageBuildingCmp.EmptySpaceCount;
            if (emptySpaceCount <= 0)
                return;

            int roomId = storageBuildingCmp.GetCenterTile().Room.Id;

            // allow all items
            if (allowedItem == null)
            {
                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;
                    if (storageBuildingCmp.Storage.Filters.Contains(item.ItemCategory))
                    {
                        AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, item);
                    }
                }
            }
            else
            {
                AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, allowedItem);
            }

            SortStorageThatHaveEmptySpaceForItemsByPriority();
        }

        private void OnStorageItemsFilterForbiddenCallback(StorageBuildingCmp storageBuildingCmp, Item forbiddenItem)
        {
            int roomId = storageBuildingCmp.GetCenterTile().Room.Id;

            // remove all items
            if (forbiddenItem == null)
            {
                RemoveStorageFromAllCollections(storageBuildingCmp);
            }
            else // remove storage only from related items collection
            {
                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;

                    bool allowed = storageBuildingCmp.IsItemAllowed(item);

                    if (allowed)
                    {
                        AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, item);
                    }
                    else
                    {
                        if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item))
                        {
                            StorageThatHaveEmptySpaceForItems[roomId][item].Remove(storageBuildingCmp.Inventory);
                        }
                    }
                }

                SortStorageThatHaveEmptySpaceForItemsByPriority();
            }
        }

        private void OnStorageCapacityChanged(StorageBuildingCmp storageBuildingCmp)
        {
            int roomId = storageBuildingCmp.GetCenterTile().Room.Id;

            int emptySpaceCount = storageBuildingCmp.EmptySpaceCount;

            if (emptySpaceCount <= 0)
            {
                Inventory storageInventory = storageBuildingCmp.Inventory;

                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;
                    if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item))
                    {
                        if (StorageThatHaveEmptySpaceForItems[roomId][item].Contains(storageInventory))
                        {
                            StorageThatHaveEmptySpaceForItems[roomId][item].Remove(storageInventory);
                        }
                    }
                }
            }
            else
            {
                foreach (var kvp in ItemDatabase.Items)
                {
                    Item item = kvp.Value;
                    if (storageBuildingCmp.Storage.Filters.Contains(item.ItemCategory) && storageBuildingCmp.IsItemAllowed(item))
                    {
                        AddStorageToStoragesThatHaveEmptySpaceForItems(storageBuildingCmp, roomId, item);
                    }
                }

                SortStorageThatHaveEmptySpaceForItemsByPriority();
            }
        }

        private void RemoveStorageFromAllCollections(StorageBuildingCmp storageBuildingCmp)
        {
            int roomId = storageBuildingCmp.GetCenterTile().Room.Id;
            Inventory storageInventory = storageBuildingCmp.Inventory;

            foreach (var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;
                if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item))
                {
                    if (StorageThatHaveEmptySpaceForItems[roomId][item].Contains(storageInventory))
                    {
                        StorageThatHaveEmptySpaceForItems[roomId][item].Remove(storageInventory);
                    }
                }
            }
        }

        public void SortStorageThatHaveEmptySpaceForItemsByPriority()
        {
            foreach (var kvp1 in StorageThatHaveEmptySpaceForItems)
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    kvp2.Value.Sort(delegate (Inventory x, Inventory y)
                    {
                        int xPriority = -(x.BuildingCmp as StorageBuildingCmp).Priority;
                        int yPriority = -(y.BuildingCmp as StorageBuildingCmp).Priority;
                        return xPriority.CompareTo(yPriority);
                    });
                }
            }
        }

        public void AddStorageToStoragesThatHaveEmptySpaceForItems(StorageBuildingCmp storageBuildingCmp, int roomId, Item item)
        {
            if (StorageThatHaveEmptySpaceForItems[roomId].ContainsKey(item) == false)
            {
                StorageThatHaveEmptySpaceForItems[roomId].Add(item, new List<Inventory>());
            }

            if (StorageThatHaveEmptySpaceForItems[roomId][item].Contains(storageBuildingCmp.Inventory) == false)
            {
                StorageThatHaveEmptySpaceForItems[roomId][item].Add(storageBuildingCmp.Inventory);
            }
        }

        public int GetItemsCount(Item item)
        {
            if (itemsCount.ContainsKey(item) == false)
                return 0;

            return itemsCount[item];
        }
        
    }
}
