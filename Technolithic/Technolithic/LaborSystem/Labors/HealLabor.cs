using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HealLabor : Labor
    {

        private HutBuildingCmp targetHut;

        public HealLabor() : base(LaborType.Heal)
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
                if (creature.AssignedHut.GetApproachableTile(creature) == null)
                    return null;

                return creature.AssignedHut;
            }

            foreach (var hut in GameplayScene.WorldManager.HutBuildingsV2)
            {
                if (hut.HasFreeSlots() == false)
                {
                    continue;
                }

                if (hut.GetApproachableTile(creature) == null)
                {
                    continue;
                }

                return hut;
            }

            return null;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            HealTask sleepTask = new HealTask(creature, targetHut);
            AddTask(creature, sleepTask);
        }

    }
}
