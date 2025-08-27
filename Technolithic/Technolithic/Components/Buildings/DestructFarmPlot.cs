namespace Technolithic
{
    public class DestructFarmPlot : BuildingCmp
    {
        public DestructFarmPlot(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager)
            : base(buildingTemplate, direction, interactablesManager)
        {
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetCenterTile();

            centerTile.GroundType = GroundType.Ground;

            DestructBuilding();
        }
    }
}
