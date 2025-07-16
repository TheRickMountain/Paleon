using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{

    public enum ActionWithAnimal
    {
        GatherProduct,
        Domesticating
    }

    public class ActionWithAnimalLabor : Labor
    {

        private AnimalCmp targetAnimal;
        private ActionWithAnimal actionWithAnimal;

        public ActionWithAnimalLabor(LaborType laborType, AnimalCmp targetAnimal, 
            ActionWithAnimal actionWithAnimal) : base(laborType)
        {
            this.targetAnimal = targetAnimal;
            this.actionWithAnimal = actionWithAnimal;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            if (targetAnimal.IsReserved)
                return false;

            // Проверяем, доступно ли животное
            if (creature.Movement.IsPathAvailable(targetAnimal.Movement.CurrentTile, true) == false)
                return false;

            Inventory inventoryToTake = null;
            Item itemToTake = null;

            if (actionWithAnimal == ActionWithAnimal.GatherProduct)
            {
                if (targetAnimal.AnimalTemplate.AnimalProduct.RequiredItem != null)
                {
                    itemToTake = targetAnimal.AnimalTemplate.AnimalProduct.RequiredItem;

                    inventoryToTake = GetStorageInventoryWithItem(itemToTake, creature);
                    if (inventoryToTake == null)
                    {
                        inventoryToTake = GetTileInventoryWithItem(itemToTake, creature);
                    }

                    if (inventoryToTake == null)
                    {
                        return false;
                    }

                    AddTask(creature, new TakeItemFromInventoryTask(creature, inventoryToTake, itemToTake, 1));
                }
            }

            AddTask(creature, new ActionWithAnimalTask(creature, targetAnimal, actionWithAnimal));

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
