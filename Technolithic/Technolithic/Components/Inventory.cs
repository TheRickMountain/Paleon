using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Inventory
    {
        private const float ALLOWED_DURABILITY_DIFFERENCE = 0.2f;

        public Dictionary<Item, List<ItemContainer>> ItemItemContainerPair = new Dictionary<Item, List<ItemContainer>>();
        public Dictionary<Item, int> InventoryToAdd { get; private set; }
        public Dictionary<Item, int> InventoryRequired { get; private set; }
        private Dictionary<Item, int> inventoryReserved;
        private Dictionary<Item, int> inventoryFactWeight;

        public int TotalItemToAddCount { get; set; }

        public Action<Inventory, Item, int> OnRequiredItemWeightChanged { get; set; }
        public Action<Inventory, Item, int> OnItemAddedCallback { get; set; }
        public Action<Inventory, Item, int> OnItemRemovedCallback { get; set; }
        public Action<Inventory, Item, int> OnItemToAddAddedCallback { get; set; }

        public Tile Tile { get; private set; }
        public BuildingCmp BuildingCmp { get; private set; }
        public CreatureCmp CreatureCmp { get; private set; }

        public int TotalItemsCount { get; private set; }
        public int TotalReservedCount { get; private set; }

        public bool IsStorage { get; set; } = true;

        public Inventory(Tile tile)
        {
            Tile = tile;
            InventoryToAdd = new Dictionary<Item, int>();
            InventoryRequired = new Dictionary<Item, int>();
            inventoryReserved = new Dictionary<Item, int>();
            inventoryFactWeight = new Dictionary<Item, int>();
        }

        public Inventory(BuildingCmp buildingCmp)
        {
            BuildingCmp = buildingCmp;
            InventoryToAdd = new Dictionary<Item, int>();
            InventoryRequired = new Dictionary<Item, int>();
            inventoryReserved = new Dictionary<Item, int>();
            inventoryFactWeight = new Dictionary<Item, int>();
        }

        public Inventory(CreatureCmp creatureCmp)
        {
            CreatureCmp = creatureCmp;
            InventoryToAdd = new Dictionary<Item, int>();
            InventoryRequired = new Dictionary<Item, int>();
            inventoryReserved = new Dictionary<Item, int>();
            inventoryFactWeight = new Dictionary<Item, int>();
        }

        public void PopItemContainer(ItemContainer itemContainer)
        {
            Item item = itemContainer.Item;
            int weight = itemContainer.FactWeight;
            List<ItemContainer> inventoryItemContainers = ItemItemContainerPair[item];

            inventoryFactWeight[item] -= itemContainer.FactWeight;

            if (IsStorage)
                GameplayScene.Instance.TotalResourcesChart.AddItem(item, -weight);

            inventoryItemContainers.Remove(itemContainer);

            TotalItemsCount -= weight;

            OnItemRemovedCallback?.Invoke(this, item, weight);

            if (IsStorage)
            {
                int factWeight = GetInventoryFactWeight(item);
                if (factWeight <= 0)
                {
                    if (GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item) == false)
                        GameplayScene.WorldManager.StoragesThatHaveItemsV2.Add(item, new List<Inventory>());

                    if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Contains(this))
                        GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Remove(this);
                }
            }
        }

        public List<ItemContainer> PopItem(Item item, int weight)
        {
            int lastWeight = weight;
            List<ItemContainer> toPopItemContainers = new List<ItemContainer>();

            List<ItemContainer> inventoryItemContainers = ItemItemContainerPair[item];

            inventoryFactWeight[item] -= weight;

            if (IsStorage)
                GameplayScene.Instance.TotalResourcesChart.AddItem(item, -weight);

            foreach (var itemContainer in inventoryItemContainers)
            {
                if(itemContainer.FactWeight > weight)
                {
                    itemContainer.FactWeight -= weight;
                    toPopItemContainers.Add(new ItemContainer(item, weight, itemContainer.Durability));
                    break;
                }
                else
                {
                    weight -= itemContainer.FactWeight;
                    toPopItemContainers.Add(new ItemContainer(item, itemContainer.FactWeight, itemContainer.Durability));
                    itemContainer.FactWeight = 0;
                }
            }

            // Удаление всех пустых itemContainer
            for (int i = inventoryItemContainers.Count - 1; i >= 0; i--)
            {
                if (inventoryItemContainers[i].FactWeight == 0)
                {
                    inventoryItemContainers.RemoveAt(i);
                }
            }

            TotalItemsCount -= lastWeight;

            OnItemRemovedCallback?.Invoke(this, item, lastWeight);

            if (IsStorage)
            {
                int factWeight = GetInventoryFactWeight(item);
                if (factWeight <= 0)
                {
                    if (GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item) == false)
                        GameplayScene.WorldManager.StoragesThatHaveItemsV2.Add(item, new List<Inventory>());

                    if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Contains(this))
                        GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Remove(this);
                }
            }

            return toPopItemContainers;
        }

        public void AddCargo(Item item, int weight)
        {
            AddCargo(new ItemContainer(item, weight, item.Durability));
        }

        public void AddCargo(Item item, int weight, float durability)
        {
            AddCargo(new ItemContainer(item, weight, durability));
        }

        public void AddCargo(ItemContainer newIC)
        {
            if (newIC == null)
                throw new ArgumentNullException();

            if (newIC.FactWeight == 0)
                return;

            Item item = newIC.Item;
            int weight = newIC.FactWeight;

            if(IsStorage)
                GameplayScene.Instance.TotalResourcesChart.AddItem(item, weight);

            if (inventoryFactWeight.ContainsKey(item) == false)
            {
                inventoryFactWeight.Add(item, weight);
            }
            else
            {
                inventoryFactWeight[item] += weight;
            }

            // Если инвентарь уже имеет предмет, то удаляем добавляемый itemContainer и прибавляем его общий вес к старому itemContainer
            if (ItemItemContainerPair.ContainsKey(item))
            {
                if (item.IsStackable)
                {
                    ItemContainer oldIC = FindItemContainerToStackWith(newIC, ItemItemContainerPair[item]);

                    if (oldIC != null)
                    {
                        oldIC.FactWeight = oldIC.FactWeight + newIC.FactWeight;

                        oldIC.Durability = CalculateAverageDurability(oldIC.FactWeight, oldIC.Durability,
                                newIC.FactWeight, newIC.Durability);
                    }
                    else
                    {
                        ItemItemContainerPair[item].Add(newIC);
                    }
                }
                else
                {
                    bool success = false;
                    foreach (var itemContainer in ItemItemContainerPair[item])
                    {
                        if (itemContainer.Durability == newIC.Durability)
                        {
                            itemContainer.FactWeight += newIC.FactWeight;
                            newIC.FactWeight = 0;
                            success = true;
                            break;
                        }
                    }

                    if (success == false)
                    {
                        ItemItemContainerPair[item].Add(newIC);
                    }
                }
            }
            else
            {
                ItemItemContainerPair.Add(item, new List<ItemContainer>() { newIC });
            }

            TotalItemsCount += weight;

            OnItemAddedCallback?.Invoke(this, item, weight);

            if (IsStorage)
            {
                int factWeight = GetInventoryFactWeight(newIC.Item);
                if (factWeight > 0)
                {
                    if (GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(newIC.Item) == false)
                        GameplayScene.WorldManager.StoragesThatHaveItemsV2.Add(newIC.Item, new List<Inventory>());

                    GameplayScene.WorldManager.StoragesThatHaveItemsV2[newIC.Item].Add(this);
                }
            }
        }

        private ItemContainer FindItemContainerToStackWith(ItemContainer ic, List<ItemContainer> icList)
        {
            for (int i = 0; i < icList.Count; i++)
            {
                ItemContainer oldIc = icList[i];

                if (CanStack(ic, oldIc, ALLOWED_DURABILITY_DIFFERENCE)) return oldIc;
            }

            return null;
        }

        private float CalculateAverageDurability(int amount1, float durability1, int amount2, float durability2)
        {
            int totalAmount = amount1 + amount2;

            return (amount1 * durability1 + amount2 * durability2) / totalAmount;
        }

        private bool CanStack(ItemContainer ic1, ItemContainer ic2, float allowedDurabilityDifference)
        {
            if (ic1.Item.IsStackable == false || ic2.Item.IsStackable == false) return false;

            float durabilityPercent1 = ic1.Durability / ic1.Item.Durability;

            float durabilityPercent2 = ic2.Durability / ic2.Item.Durability;

            float durabilityDiff = Math.Abs(durabilityPercent1 - durabilityPercent2);

            return durabilityDiff <= allowedDurabilityDifference;
        }

        public void ReserveItemWeight(Item item, int weight)
        {
            TotalReservedCount += weight;

            if (inventoryReserved.ContainsKey(item) == false)
                inventoryReserved.Add(item, weight);
            else
                inventoryReserved[item] += weight;

            int availableWeight = GetAvailableItemCount(item);
            int roomId = GetRoom() == null ? -1 : GetRoom().Id;

            if(Tile != null)
            {
                if (availableWeight <= 0)
                {
                    if (GameplayScene.WorldManager.TilesThatHaveItems[roomId].ContainsKey(item))
                        GameplayScene.WorldManager.TilesThatHaveItems[roomId][item].Remove(this);
                }
                else
                {
                    if (GameplayScene.WorldManager.TilesThatHaveItems[roomId].ContainsKey(item) == false)
                        GameplayScene.WorldManager.TilesThatHaveItems[roomId].Add(item, new List<Inventory>());

                    if (GameplayScene.WorldManager.TilesThatHaveItems[roomId][item].Contains(this) == false)
                        GameplayScene.WorldManager.TilesThatHaveItems[roomId][item].Add(this);
                }
            }
        }

        public int GetInventoryReservedWeight(Item item)
        {
            if (!inventoryReserved.ContainsKey(item))
                return 0;
            else
                return inventoryReserved[item];
        }

        public int GetInventoryRequiredWeight(Item item)
        {
            if (!InventoryRequired.ContainsKey(item))
                return 0;
            else
                return InventoryRequired[item];
        }

        public int GetInventoryToAddWeight(Item item)
        {
            if (!InventoryToAdd.ContainsKey(item))
                return 0;
            else
                return InventoryToAdd[item];
        }

        public virtual int GetInventoryRequiredMinusToAddWeight(Item item)
        {
            int inventoryRequiredWeight = GetInventoryRequiredWeight(item);

            int inventoryToAddWeight = GetInventoryToAddWeight(item);

            return inventoryRequiredWeight - inventoryToAddWeight;
        }

        public int GetInventoryFactWeight(Item item)
        {
            if (!inventoryFactWeight.ContainsKey(item))
                return 0;

            return inventoryFactWeight[item];
        }

        public int GetAvailableItemCount(Item item)
        {
            int factWeight = GetInventoryFactWeight(item);
            int reservedWeight = GetInventoryReservedWeight(item);

            return factWeight - reservedWeight;
        }

        public void AddRequiredWeight(Item item, int weight)
        {
            if (weight == 0)
                return;

            if (!InventoryRequired.ContainsKey(item))
                InventoryRequired.Add(item, weight);
            else
                InventoryRequired[item] += weight;

            if(BuildingCmp != null)
            {
                if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2.ContainsKey(item) == false)
                    GameplayScene.WorldManager.BuildingsThatRequireItemsV2.Add(item, new List<Inventory>());

                if(InventoryRequired[item] > 0)
                {
                    if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Contains(this) == false)
                        GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Add(this);
                }
                else
                {
                    if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Contains(this))
                        GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Remove(this);
                }
            }

            OnRequiredItemWeightChanged?.Invoke(this, item, weight);
        }

        public void AddToAddWeight(Item item, int weight)
        {
            if (weight == 0)
                return;

            if (!InventoryToAdd.ContainsKey(item))
                InventoryToAdd.Add(item, weight);
            else
                InventoryToAdd[item] += weight;

            TotalItemToAddCount += weight;

            OnItemToAddAddedCallback?.Invoke(this, item, weight);
        }

        public void ClearCargo()
        {
            foreach (var kvp in ItemItemContainerPair)
            {
                Item item = kvp.Key;
                int factWeight = GetInventoryFactWeight(item);
                if (factWeight == 0)
                    continue;

                foreach (var itemContainer in PopItem(item, factWeight))
                {
                   
                }
            }

            ItemItemContainerPair.Clear();
        }

        public void ThrowCargo(Tile tile)
        {
            foreach(var kvp in ItemItemContainerPair)
            {
                Item item = kvp.Key;
                int factWeight = GetInventoryFactWeight(item);
                if (factWeight == 0)
                    continue;
                    
                foreach (var itemContainer in PopItem(item, factWeight))
                {
                    tile.Inventory.AddCargo(itemContainer);
                }
            }

            ItemItemContainerPair.Clear();
        }

        public void ThrowCargo(Tile tile, Item item, int weight)
        {
            foreach (var itemContainer in PopItem(item, weight))
            {
                tile.Inventory.AddCargo(itemContainer);
            }
        }

        public IEnumerable<KeyValuePair<Item, List<ItemContainer>>> GetCargoEnumerable()
        {
            return ItemItemContainerPair.AsEnumerable();
        }

        public Tile GetReachableTile(CreatureCmp creatureCmp)
        {
            if(Tile != null && Tile.Room != null && Tile.Room.Id == creatureCmp.Movement.CurrentTile.Room.Id)
            {
                return Tile;
            }

            if(BuildingCmp != null)
            {
                return BuildingCmp.GetReachableTile(creatureCmp);
            }

            return null;
        }

        public Room GetRoom()
        {
            if (Tile != null)
            {
                return Tile.Room;
            }

            if (BuildingCmp != null)
            {
                return BuildingCmp.GetCenterTile().Room;
            }

            return null;
        }

        public string GetInformation(Item item)
        {
            return CreateInformation(item);
        }

        private string CreateInformation(Item item)
        {
            if (item.Durability > 0)
            {
                string info;

                info = $"\n{Localization.GetLocalizedText("durability")}:";

                List<ItemContainer> itemContainers = ItemItemContainerPair[item];

                foreach(var itemContainer in itemContainers)
                {
                    info += "\n" + itemContainer.GetDurabilityInfo();
                }

                return info;
            }

            return "";
        }
    }
}
