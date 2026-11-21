using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class HaulLabor : Labor
    {

        public HaulLabor() : base(LaborType.Haul)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            Item itemToTake = null;
            Inventory takeFromPile = null;
            StorageBuildingCmp putToStorage = null;
            int creatureRoomId = creature.Movement.CurrentTile.Room.Id;

            foreach(var kvp in GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId])
            {
                List<Inventory> tilesInventories = kvp.Value;

                if (tilesInventories.Count == 0)
                    continue;

                Item item = kvp.Key;

                if (GameplayScene.WorldManager.StorageManager.StorageThatHaveEmptySpaceForItems[creatureRoomId].ContainsKey(item) == false)
                    continue;

                if (GameplayScene.WorldManager.StorageManager.StorageThatHaveEmptySpaceForItems[creatureRoomId][item].Count == 0)
                    continue;

                foreach(var storage in GameplayScene.WorldManager.StorageManager.StorageThatHaveEmptySpaceForItems[creatureRoomId][item])
                {
                    if((storage.BuildingCmp as StorageBuildingCmp).EmptySpaceCount > 0)
                    {
                        putToStorage = storage.BuildingCmp as StorageBuildingCmp;
                        break;
                    }
                }

                if (putToStorage == null)
                    continue;

                itemToTake = item;
                takeFromPile = tilesInventories[0];
                break;
            }

            if (itemToTake == null || takeFromPile == null || putToStorage == null)
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

            // Вычисляем, сколько предметов может взять поселенец
            int tileAvailableWeight = takeFromPile.GetAvailableItemCount(itemToTake);

            int weightToTake;

            if(tileAvailableWeight > 4)
            {
                weightToTake = 4;
            }
            else
            {
                weightToTake = tileAvailableWeight;
            }

            // Вычисляем сколько веса может вместить склад
            int storageEmptySpaceCount = putToStorage.EmptySpaceCount;

            if(storageEmptySpaceCount < weightToTake)
            {
                weightToTake = storageEmptySpaceCount;
            }

            HaulTask haulTask = new HaulTask(creature, putToStorage, takeFromPile, itemToTake, weightToTake);
            AddTask(creature, haulTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
