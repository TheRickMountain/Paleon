using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DestructIrrigationCanalCmp : BuildingCmp
    {

        public DestructIrrigationCanalCmp(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetCenterTile();

            GameplayScene.Instance.DestructIrrigationCanal(centerTile);

            DestructBuilding();
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();
        }

    }
}