using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GatherLabor : Labor
    {

        public GatherLabor() : base(LaborType.Gather)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            DepositCmp targetDeposit = null;

            for (int i = 0; i < GameplayScene.WorldManager.GatherableDeposits.Count; i++)
            {
                DepositCmp depositCmp = GameplayScene.WorldManager.GatherableDeposits[i];
                if(depositCmp.GetReachableTile(creature) != null && depositCmp.IsReserved == false &&
                    depositCmp.IsMarkedToObtain == true && depositCmp.CanBeObtained())
                {
                    targetDeposit = depositCmp;
                }
            }

            if (targetDeposit == null)
                return false;

            MineTask gatherTask = new MineTask(creature, targetDeposit);
            AddTask(creature, gatherTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
