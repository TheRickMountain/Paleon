using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GetDressedLabor : Labor
    {

        private Item itemToTake;
        private Inventory takeFromInventory;
        private bool equipTop;

        public GetDressedLabor(bool equipTop) : base(LaborType.Equip)
        {
            this.equipTop = equipTop;
        }

        public override bool Check(CreatureCmp creatureCmp)
        {
            if (CreatureTasks.Count > 0)
                return false;

            var tuplePair = FindClothing(creatureCmp);
            if (tuplePair == null)
                return false;

            takeFromInventory = tuplePair.Item1;
            itemToTake = tuplePair.Item2;

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            EquipItemTask getDressedTask = new EquipItemTask(creature, takeFromInventory, itemToTake);
            AddTask(creature, getDressedTask);
        }

        private Tuple<Inventory, Item> FindClothing(CreatureCmp creatureCmp)
        {
            int creatureRoomId = creatureCmp.Movement.CurrentTile.Room.ZoneId;

            List<Item> clothesList = null;

            if(equipTop)
            {
                clothesList = ItemDatabase.TopClothes;
            }
            else
            {
                clothesList = ItemDatabase.Clothes;
            }

            // Сначала ищем на складе
            foreach (var item in clothesList)
            {
                StorageBuildingCmp storageBuildingCmp = GameplayScene.WorldManager.StorageManager.GetStorageWith(creatureCmp, item);
                if(storageBuildingCmp != null)
                {
                    return Tuple.Create(storageBuildingCmp.Inventory, item);
                }
            }

            if (GameplayScene.WorldManager.TilesThatHaveItems[creatureRoomId].Count != 0)
            {
                // Теперь ищем на тайлах
                foreach (var item in clothesList)
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
