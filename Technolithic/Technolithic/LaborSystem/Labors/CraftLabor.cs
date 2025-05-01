using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CraftLabor : Labor
    {
        public CraftLabor(LaborType laborType) : base(laborType)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            CrafterBuildingCmp targetCrafter = null;

            for (int i = 0; i < GameplayScene.WorldManager.CrafterBuildings[LaborType].Count; i++)
            {
                CrafterBuildingCmp crafter = GameplayScene.WorldManager.CrafterBuildings[LaborType][i];
             
                if(crafter.Crafter.CreatureType != creature.CreatureType)
                {
                    continue;
                }
                
                if (crafter.GetReachableTile(creature) != null && crafter.IsReserved == false && crafter.CanCraft)
                {
                    targetCrafter = crafter;
                    break;
                }
            }

            if (targetCrafter == null)
                return false;

            CraftTask craftTask = new CraftTask(creature, targetCrafter);
            AddTask(creature, craftTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}