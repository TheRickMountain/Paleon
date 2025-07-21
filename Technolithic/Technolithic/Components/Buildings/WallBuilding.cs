namespace Technolithic
{
    public class WallBuilding : BuildingCmp
    {
        public WallBuilding(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager)
            : base(buildingTemplate, direction, interactablesManager)
        {
            ThrowBuildingRecipeItemsAfterDestructing = false;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetRealCenterTile();

            centerTile.WallId = BuildingTemplate.WallData.Id;

            DestructBuilding();
        }
    }
}
