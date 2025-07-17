using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LighterCmp : BuildingCmp
    {

        public LighterCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = IsPowered;
        }

    }
}
