using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MineLabor : Labor
    {

        public MineLabor() : base(LaborType.Mine)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            DepositCmp targetDeposit = null;

            for (int i = 0; i < GameplayScene.WorldManager.MineableDeposits.Count; i++)
            {
                DepositCmp depositCmp = GameplayScene.WorldManager.MineableDeposits[i];
                if(depositCmp.GetApproachableTile(creature) != null && depositCmp.IsReserved == false &&
                    depositCmp.IsMarkedToObtain == true && depositCmp.CanBeObtained())
                {
                    targetDeposit = depositCmp;
                    break;
                }
            }

            if (targetDeposit == null)
                return false;

            ToolType requiredToolType = ToolType.Pick;

            if (requiredToolType != ToolType.None)
            {
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
            }

            MineTask mineTask = new MineTask(creature, targetDeposit);
            AddTask(creature, mineTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
