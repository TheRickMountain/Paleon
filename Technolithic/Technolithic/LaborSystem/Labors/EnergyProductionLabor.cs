using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EnergyProductionLabor : Labor
    {

        public EnergyProductionLabor() : base(LaborType.EnergyProduction)
        {
            
        }

        public override bool Check(CreatureCmp creature)
        {
            CreaturePoweredEnergySource targetEnergySource = null;
        
            foreach(CreaturePoweredEnergySource energySource in GameplayScene.WorldManager.CreaturePoweredEnergySources)
            {
                if(energySource.IsBuilt == false)
                    continue;

                if (energySource.IsReserved)
                    continue;

                if (energySource.GetApproachableTile(creature) == null)
                    continue;

                targetEnergySource = energySource;
                break;
            }

            if (targetEnergySource == null)
            {
                return false;
            }

            EnergyProductionTask energyProductionTask = new EnergyProductionTask(creature, targetEnergySource);
            AddTask(creature, energyProductionTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
