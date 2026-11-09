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

        public static ConstructionSiteCmp GetConstructionSite(this Entity entity)
        {
            return entity?.Get<ConstructionSiteCmp>();
        }

        public static BuildingPlacement GetBuildingPlacement(this Entity entity)
        {
            return entity?.Get<BuildingCmp>()?.Placement ?? entity.GetConstructionSite()?.Placement;
        }

        public static BuildingSaveData GetBuildingSaveData(this Entity entity)
        {
            return entity?.Get<BuildingCmp>()?.GetSaveData() ?? entity.GetConstructionSite()?.GetSaveData();
        }
    }
}
