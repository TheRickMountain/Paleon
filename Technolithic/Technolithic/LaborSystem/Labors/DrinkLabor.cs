using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DrinkLabor : Labor
    {

        private Item itemToTake;
        private Inventory takeFromInventory;

        public DrinkLabor() : base(LaborType.Drink)
        {

        }

        public override bool Check(CreatureCmp creatureCmp)
        {
            if (CreatureTasks.Count > 0)
                return false;

            var tuplePair = FindBeverage(creatureCmp);
            if (tuplePair == null)
                return false;

            takeFromInventory = tuplePair.Item1;
            itemToTake = tuplePair.Item2;

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            ConsumeTask drink = new ConsumeTask(creature, takeFromInventory, itemToTake);
            AddTask(creature, drink);
        }

        private Tuple<Inventory, Item> FindBeverage(CreatureCmp creatureCmp)
        {
            int creatureRoomId = creatureCmp.Movement.CurrentTile.Room.Id;

            // Сначала ищем на складе
            foreach (var item in creatureCmp.BeverageRation.GetAllowedRation())
            {
                StorageBuildingCmp storageBuildingCmp = GameplayScene.WorldManager.StorageManager.GetStorageWith(creatureCmp, item);
                if (storageBuildingCmp != null)
                {
                    return Tuple.Create(storageBuildingCmp.Inventory, item);
                }
            }

            if (GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId].Count != 0)
            {
                // Теперь ищем на тайлах
                foreach (var item in creatureCmp.BeverageRation.GetAllowedRation())
                {
                    if (!GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId].ContainsKey(item))
                        continue;

                    if (GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId][item].Count == 0)
                        continue;

                    List<Inventory> inventories = GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId][item];

                    return Tuple.Create(inventories[0], item);
                }
            }

            return null;
        }

    }
}
