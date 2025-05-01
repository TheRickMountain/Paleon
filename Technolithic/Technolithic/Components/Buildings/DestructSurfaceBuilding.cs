using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DestructSurfaceBuilding : BuildingCmp
    {

        public DestructSurfaceBuilding(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetRealCenterTile();

            int surfaceId = centerTile.SurfaceId;

            BuildingTemplate surfaceBuildingTemplate = Engine.Instance.SurfaceIdBuildingTemplate[surfaceId];

            ThrowItems(surfaceBuildingTemplate.BuildingRecipe);

            centerTile.SurfaceId = -1;

            DestructBuilding();
        }

    }
}
