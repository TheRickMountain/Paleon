namespace Technolithic
{
    public class DestructWallBuilding : BuildingCmp
    {

        public DestructWallBuilding(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager)
            : base(buildingTemplate, direction, interactablesManager)
        {

        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile centerTile = GetRealCenterTile();

            int wallId = centerTile.WallId;

            BuildingTemplate buildingTemplate = Engine.Instance.WallIdBuildingTemplate[wallId];

            centerTile.WallId = -1;

            ThrowItems(buildingTemplate.ConstructionData.RealIngredients);

            DestructBuilding();
        }

    }
}
