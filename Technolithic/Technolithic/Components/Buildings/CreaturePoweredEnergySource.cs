using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CreaturePoweredEnergySource : EnergySourceCmp
    {

        public CreatureCmp CurrentCreature { get; set; }

        public CreaturePoweredEnergySource(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            GameplayScene.WorldManager.CreaturePoweredEnergySources.Add(this);
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.WorldManager.CreaturePoweredEnergySources.Remove(this);
        }

        public override int GetActualEnergyOutput()
        {
            if (CurrentCreature != null)
            {
                // TODO: Probably it would be better settler to product less energy than animals
                return BuildingTemplate.EnergySourceData.GeneratedPower;
            }

            return 0;
        }
    }
}
