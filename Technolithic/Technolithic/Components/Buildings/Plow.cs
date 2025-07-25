namespace Technolithic
{
    public class Plow : BuildingCmp
    {
        public Plow(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetCenterTile();

            centerTile.GroundType = GroundType.FarmPlot;

            DestructBuilding();
        }
    }
}
