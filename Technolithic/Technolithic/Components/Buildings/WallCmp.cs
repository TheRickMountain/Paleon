using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WallCmp : BuildingCmp
    {

        public WallCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            
        }
    }
}
