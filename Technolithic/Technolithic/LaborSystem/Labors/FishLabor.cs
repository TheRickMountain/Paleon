using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class FishLabor : Labor
    {

        private Item fishItem;

        public FishLabor() : base(LaborType.Fish)
        {
            fishItem = ItemDatabase.GetItemByName("raw_fish");
        }

        public override bool Check(CreatureCmp creature)
        {
            if (GameplayScene.WorldManager.FishingPlaceBuildings.Count == 0)
                return false;

            FishingPlaceCmp targetFishingPlaceCmp = null;

            foreach(var fishingPlace in GameplayScene.WorldManager.FishingPlaceBuildings)
            {
                if(fishingPlace.IsReserved == false && fishingPlace.GetReachableTile(creature) != null
                    && fishingPlace.IsWaterChunkHasFish())
                {
                    targetFishingPlaceCmp = fishingPlace;
                    break;
                }
            }

            if (targetFishingPlaceCmp == null)
                return false;

            ToolType requiredToolType = ToolType.Fishing;

            if (creature.CreatureEquipment.HasTool(requiredToolType) == false)
            {
                var tuplePair = GameplayScene.WorldManager.FindTool(creature, requiredToolType);

                if (tuplePair?.Item1 != null)
                {
                    Inventory inventory = tuplePair.Item1;
                    Item item = tuplePair.Item2;

                    EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                    AddTask(creature, equipTask);
                }
                else
                {
                    return false;
                }
            }

            AddTask(creature, new FishTask(creature, targetFishingPlaceCmp));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
