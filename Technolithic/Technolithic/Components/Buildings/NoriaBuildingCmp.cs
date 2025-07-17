using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class NoriaBuildingCmp : BuildingCmp
    {
        private Tile centerTile;
        private Tile directionTile;

        public NoriaBuildingCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {

        }

        private Tile GetTileInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN: return centerTile.BottomTile;
                case Direction.UP: return centerTile.TopTile;
                case Direction.LEFT: return centerTile.LeftTile;
                case Direction.RIGHT: return centerTile.RightTile;
                default: throw new ArgumentException("Invalid direction");
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            centerTile = GetRealCenterTile();
            directionTile = GetTileInDirection(Direction);

            centerTile.GroundTopType = GroundTopType.Water;
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = IsPowered;

            if (IsPowered)
            {
                if (directionTile.GroundTopType != GroundTopType.IrrigationCanalFull
                    && directionTile.GroundTopType != GroundTopType.IrrigationCanalEmpty)
                {
                    GameplayScene.Instance.BuildIrrigationCanal(directionTile);
                }
            }
            else
            {
                if(directionTile.GroundTopType != GroundTopType.None)
                {
                    GameplayScene.Instance.DestructIrrigationCanal(directionTile);
                }
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            centerTile.GroundTopType = GroundTopType.None;

            GameplayScene.Instance.DestructIrrigationCanal(directionTile);
        }

    }
}
