using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PrepareCrafterLabor : Labor
    {

        public PrepareCrafterLabor(LaborType laborType) : base(laborType)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            CrafterBuildingCmp targetCrafter = null;

            for (int i = 0; i < GameplayScene.WorldManager.AutoCrafterBuildings[LaborType].Count; i++)
            {
                CrafterBuildingCmp crafter = GameplayScene.WorldManager.AutoCrafterBuildings[LaborType][i];
                if (crafter.GetApproachableTile(creature) != null &&
                    crafter.CanCraft &&
                    crafter.IsPrepared == false && 
                    crafter.IsReserved == false)
                {
                    targetCrafter = crafter;
                    break;
                }
            }

            if (targetCrafter == null)
                return false;

            AddTask(creature, new PrepareCrafterTask(creature, targetCrafter));
            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
