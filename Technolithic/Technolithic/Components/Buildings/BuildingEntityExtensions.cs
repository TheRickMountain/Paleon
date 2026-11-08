namespace Technolithic
{
    public static class BuildingEntityExtensions
    {
        public static T GetCompletedBuilding<T>(this Entity entity) where T : BuildingCmp
        {
            T building = entity?.Get<T>();

            return building != null && building.IsBuilt ? building : null;
        }

        public static BuildingCmp GetCompletedBuilding(this Entity entity)
        {
            return entity.GetCompletedBuilding<BuildingCmp>();
        }

        public static BuildingCmp GetConstructionSite(this Entity entity)
        {
            BuildingCmp building = entity?.Get<BuildingCmp>();

            return building != null && building.IsBuilt == false ? building : null;
        }

        public static BuildingPlacement GetBuildingPlacement(this Entity entity)
        {
            return entity?.Get<BuildingCmp>()?.Placement;
        }
    }
}
