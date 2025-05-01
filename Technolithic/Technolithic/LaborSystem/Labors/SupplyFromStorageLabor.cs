using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class SupplyFromStorageLabor : Labor
    {

        public SupplyFromStorageLabor() : base(LaborType.Supply)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            Item itemToTake = null;
            StorageBuildingCmp takeFromStorage = null;
            BuildingCmp putToBuilding = null;

            foreach (var kvp in GameplayScene.WorldManager.StoragesThatHaveItemsV2)
            {
                Item item = kvp.Key;

                takeFromStorage = null;
                putToBuilding = null;

                if (GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item) == false)
                    continue;

                if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Count == 0)
                    continue;

                if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2.ContainsKey(item) == false)
                    continue;

                if (GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item].Count == 0)
                    continue;

                foreach (var storageInventory in GameplayScene.WorldManager.StoragesThatHaveItemsV2[item])
                {
                    if(storageInventory.GetAvailableItemCount(item) > 0 && storageInventory.GetReachableTile(creature) != null)
                    {
                        takeFromStorage = storageInventory.BuildingCmp as StorageBuildingCmp;
                        break;
                    }
                }

                if (takeFromStorage == null)
                    continue;

                foreach(var buildingThatMaybeRequireItem in GameplayScene.WorldManager.BuildingsThatRequireItemsV2[item])
                {
                    if (buildingThatMaybeRequireItem.GetInventoryRequiredMinusToAddWeight(item) > 0 &&
                            buildingThatMaybeRequireItem.GetReachableTile(creature) != null)
                    {
                        putToBuilding = buildingThatMaybeRequireItem.BuildingCmp;
                        break;
                    }
                }

                if (putToBuilding == null)
                    continue;

                itemToTake = item;
                break;
            }

            if (itemToTake == null || takeFromStorage == null || putToBuilding == null)
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

            int storageAvailableWeight = takeFromStorage.Inventory.GetAvailableItemCount(itemToTake);

            int weightToTake;

            if(storageAvailableWeight > 4)
            {
                weightToTake = 4;
            }
            else
            {
                weightToTake = storageAvailableWeight;
            }

            int buildingRequiredWeight = putToBuilding.Inventory.GetInventoryRequiredMinusToAddWeight(itemToTake);

            if (buildingRequiredWeight < weightToTake)
            {
                weightToTake = buildingRequiredWeight;
            }

            if(weightToTake <= 0)
            {
                throw new Exception("Weight to take can't be lower than 0!");
            }

            SupplyFromStorageTask supplyTask = new SupplyFromStorageTask(creature, takeFromStorage, putToBuilding, itemToTake, weightToTake);
            AddTask(creature, supplyTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
