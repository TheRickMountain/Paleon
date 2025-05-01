using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EmptyFishTrapLabor : Labor
    {

        public EmptyFishTrapLabor() : base(LaborType.Fish)
        {
            
        }

        public override bool Check(CreatureCmp creature)
        {
            if (GameplayScene.WorldManager.FishTrapBuildings.Count == 0)
                return false;

            FishTrap targetFishTrap = null;

            foreach (var fishTrap in GameplayScene.WorldManager.FishTrapBuildings)
            {
                if (fishTrap.IsReserved == false && fishTrap.CatchedItem != null && fishTrap.GetReachableTile(creature) != null)
                {
                    targetFishTrap = fishTrap;
                    break;
                }
            }

            if (targetFishTrap == null)
                return false;

            AddTask(creature, new EmptyFishTrapTask(creature, targetFishTrap));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
