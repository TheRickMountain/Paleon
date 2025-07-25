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

        public CreaturePoweredEnergySource(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
            
        }

        public override void ProcessInteraction(InteractionType interactionType, CreatureCmp creature)
        {
            base.ProcessInteraction(interactionType, creature);

            switch(interactionType)
            {
                case InteractionType.ProduceEnergy:
                    {
                        CurrentCreature = creature;
                    }
                    break;
            }
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            CurrentCreature = null;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            AddAvailableInteraction(InteractionType.ProduceEnergy, LaborType.EnergyProduction);
            ActivateInteraction(InteractionType.ProduceEnergy);
            SetInteractionDuration(InteractionType.ProduceEnergy, 0);
            MarkInteraction(InteractionType.ProduceEnergy);
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
