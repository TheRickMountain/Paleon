using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class SupplyFromTileLabor : Labor
    {

        public SupplyFromTileLabor() : base(LaborType.Supply)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            Item itemToTake = null;
            Tile takeFromTile = null;
            BuildingCmp putToBuilding = null;
            int creatureRoomId = creature.Movement.CurrentTile.Room.ZoneId;

            foreach (var kvp in GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId])
            {
                List<Inventory> tilesInventories = kvp.Value;

                if (tilesInventories.Count == 0)
                    continue;

                Item item = kvp.Key;

                if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2.ContainsKey(item) == false)
                    continue;

                if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Count == 0)
                    continue;

                foreach (var buildingThatMaybeRequireItem in GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item])
                {
                    if (buildingThatMaybeRequireItem.GetReachableTile(creature) != null)
                    {
                        if (buildingThatMaybeRequireItem.GetInventoryRequiredMinusToAddWeight(item) > 0)
                        {
                            putToBuilding = buildingThatMaybeRequireItem.BuildingCmp;
                            break;
                        }
                    }
                }

                if (putToBuilding == null)
                    continue;

                itemToTake = item;
                takeFromTile = tilesInventories[0].Tile;
                break;
            }

            if (itemToTake == null || takeFromTile == null || putToBuilding == null)
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

            int tileAvailableWeight = takeFromTile.Inventory.GetAvailableItemCount(itemToTake);

            int weightToTake;

            if (tileAvailableWeight > 4)
            {
                weightToTake = 4;
            }
            else
            {
                weightToTake = tileAvailableWeight;
            }

            int buildingRequiredWeight = putToBuilding.Inventory.GetInventoryRequiredMinusToAddWeight(itemToTake);

            if (buildingRequiredWeight < weightToTake)
            {
                weightToTake = buildingRequiredWeight;
            }

            if (weightToTake <= 0)
            {
                throw new Exception("Weight to take can't be lower than 0!");
            }

            SupplyFromTileTask supplyTask = new SupplyFromTileTask(creature, takeFromTile, putToBuilding, itemToTake, weightToTake);
            AddTask(creature, supplyTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
