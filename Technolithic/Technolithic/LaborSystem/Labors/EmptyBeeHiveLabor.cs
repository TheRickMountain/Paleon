using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EmptyBeeHiveLabor : Labor
    {

        public EmptyBeeHiveLabor() : base(LaborType.Beekeeping)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (GameplayScene.WorldManager.BeeHiveBuildings.Count == 0)
                return false;

            BeeHiveBuildingCmp targetBeeHive = null;

            foreach(var beeHive in GameplayScene.WorldManager.BeeHiveBuildings)
            {
                if(beeHive.GatherResources && beeHive.IsReserved == false && 
                    beeHive.IsEmpty == false && beeHive.GetApproachableTile(creature) != null)
                {
                    targetBeeHive = beeHive;
                    break;
                }
            }

            if (targetBeeHive == null)
                return false;

            AddTask(creature, new EmptyBeeHiveTask(creature, targetBeeHive));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }

    }
}
