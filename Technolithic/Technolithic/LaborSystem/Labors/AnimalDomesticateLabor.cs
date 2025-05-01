using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{

    public class AnimalDomesticateLabor : Labor
    {

        public AnimalDomesticateLabor() : base(LaborType.Ranching)
        {
        }

        public override bool Check(CreatureCmp creature)
        {
            AnimalCmp targetAnimal = null;

            foreach(var animal in GameplayScene.WorldManager.AnimalsToDomesticate)
            {
                if (animal.IsReserved)
                    continue;

                if (animal.IsDomesticated)
                    continue;

                if (creature.Movement.CurrentTile.GetRoomId() != animal.Movement.CurrentTile.GetRoomId())
                    continue;

                targetAnimal = animal;
                break;
            }

            if (targetAnimal == null)
                return false;

            Inventory inventoryToTake = null;
            Item itemToTake = null;

            foreach (var checkItem in targetAnimal.AnimalTemplate.Ration)
            {
                itemToTake = checkItem;

                inventoryToTake = GetStorageInventoryWithItem(itemToTake, creature);
                if (inventoryToTake == null)
                {
                    inventoryToTake = GetTileInventoryWithItem(itemToTake, creature);
                }

                if (inventoryToTake != null)
                {
                    break;
                }
            }

            if (inventoryToTake == null)
            {
                return false;
            }

            AddTask(creature, new TakeItemFromInventoryTask(creature, inventoryToTake, itemToTake, 1));

            AddTask(creature, new ActionWithAnimalTask(creature, targetAnimal, ActionWithAnimal.Domesticating));

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

            foreach (var inventory in GameplayScene.WorldManager.StoragesThatHaveItemsV2[item])
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
