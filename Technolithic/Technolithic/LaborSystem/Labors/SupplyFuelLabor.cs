using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SupplyFuelLabor : Labor
    {

        public SupplyFuelLabor() : base(LaborType.Supply)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (GameplayScene.WorldManager.FuelConsumerBuildings.Count == 0)
                return false;

            BuildingCmp buildingToSupply = null;
            Inventory inventoryToTake = null;
            Item itemToSupply = null;

            foreach(var building in GameplayScene.WorldManager.FuelConsumerBuildings)
            {
                // Уже кто-то другой собирается доставить топливо в строение
                if (building.ReservedToSupplyFuel)
                    continue;

                if (building.IsBuilt == false)
                    continue;

                if (building.GetApproachableTile(creature) == null)
                    continue;

                if (building.CurrentFuelCondition > 0)
                    continue;

                foreach(var kvp in building.ConsumableFuelDictionary)
                {
                    if(kvp.Value)
                    {
                        inventoryToTake = GetStorageInventoryWithItem(kvp.Key, creature);
                        if(inventoryToTake == null)
                        {
                            inventoryToTake = GetTileInventoryWithItem(kvp.Key, creature);
                        }

                        if (inventoryToTake != null)
                        {
                            itemToSupply = kvp.Key;
                            buildingToSupply = building;
                            break;
                        }
                    }
                }

                if (buildingToSupply != null && inventoryToTake != null && itemToSupply != null)
                    break;
            }

            if (buildingToSupply == null || inventoryToTake == null || itemToSupply == null)
                return false;

            if (creature.CreatureEquipment.HasTool(ToolType.Hauling) == false)
            {
                var tuplePair = GameplayScene.WorldManager.FindTool(creature, ToolType.Hauling);

                if (tuplePair?.Item1 != null)
                {
                    Inventory inventory = tuplePair.Item1;
                    Item item = tuplePair.Item2;

                    EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                    AddTask(creature, equipTask);
                }
                else if (creature.CreatureType == CreatureType.Animal)
                {
                    return false;
                }
            }

            AddTask(creature, new TakeItemFromInventoryToSupplyTask(creature, inventoryToTake, itemToSupply, 1, buildingToSupply));
            AddTask(creature, new SupplyFuelTask(creature, buildingToSupply, itemToSupply));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

        private Inventory GetStorageInventoryWithItem(Item item, CreatureCmp creatureCmp)
        {
            int roomId = creatureCmp.Movement.CurrentTile.Room.Id;

            if (!GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item))
                return null;

            if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Count == 0)
                return null;

            foreach(var inventory in GameplayScene.WorldManager.StoragesThatHaveItemsV2[item])
            {
                if (inventory.GetAvailableItemCount(item) > 0 && inventory.GetReachableTile(creatureCmp) != null)
                    return inventory;
            }

            return null;
        }

        private Inventory GetTileInventoryWithItem(Item item, CreatureCmp creatureCmp)
        {
            int roomId = creatureCmp.Movement.CurrentTile.Room.Id;

            if (!GameplayScene.WorldManager.TilesThatHaveItems[roomId].ContainsKey(item))
                return null;

            if (GameplayScene.WorldManager.TilesThatHaveItems[roomId][item].Count == 0)
                return null;

            foreach (var inventory in GameplayScene.WorldManager.TilesThatHaveItems[roomId][item])
            {
                if (inventory.GetAvailableItemCount(item) > 0)
                    return inventory;
            }

            return null;
        }
    }
}
