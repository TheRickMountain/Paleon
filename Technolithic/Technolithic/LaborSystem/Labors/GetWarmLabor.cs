using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GetWarmLabor : Labor
    {

        private HutBuildingCmp targetHut;

        public GetWarmLabor() : base(LaborType.Heal)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            targetHut = TryGetTargetHut(creature);

            return targetHut != null;
        }

        private HutBuildingCmp TryGetTargetHut(CreatureCmp creature)
        {
            if (creature.AssignedHut != null)
            {
                if (creature.AssignedHut.GetReachableTile(creature) == null)
                    return null;

                if (creature.AssignedHut.CurrentFuelCondition <= 0)
                    return null;

                return creature.AssignedHut;
            }

            foreach (var hut in GameplayScene.WorldManager.HutBuildingsV2)
            {
                if (hut.HasFreeSlots() == false)
                {
                    continue;
                }

                if (hut.GetReachableTile(creature) == null)
                {
                    continue;
                }

                if(hut.CurrentFuelCondition <= 0)
                {
                    continue;
                }

                return hut;
            }

            return null;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            HealTask healTask = new HealTask(creature, targetHut);
            AddTask(creature, healTask);
        }

    }
}
