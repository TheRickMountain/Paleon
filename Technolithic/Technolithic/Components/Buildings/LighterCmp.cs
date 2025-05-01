using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LighterCmp : BuildingCmp
    {

        public LighterCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = IsPowered;
        }

    }
}
