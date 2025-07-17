using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SurfaceBuilding : BuildingCmp
    {

        public SurfaceBuilding(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
            ThrowBuildingRecipeItemsAfterDestructing = false;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetRealCenterTile();

            centerTile.SurfaceId = BuildingTemplate.SurfaceData.Id;

            DestructBuilding();
        }

    }
}
