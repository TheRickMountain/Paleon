namespace Technolithic
{
    public class BuildIrrigationCanalCmp : BuildingCmp
    {

        public BuildIrrigationCanalCmp(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetCenterTile();

            if (centerTile.GroundType == GroundType.Grass)
            {
                centerTile.GroundType = GroundType.Ground;
            }

            GameplayScene.Instance.BuildIrrigationCanal(centerTile);

            DestructBuilding();
        }

    }
}