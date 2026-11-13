using System;

namespace Technolithic
{
    public class BuildingDestructFilter : DestructFilter
    {
        public BuildingDestructFilter(string id, string nameKey, Func<MyTexture> getIcon, bool isEnabledByDefault)
            : base(id, nameKey, getIcon, isEnabledByDefault)
        {
        }

        public override bool Covers(Tile tile)
        {
            BuildingCmp building = tile.Entity.GetCompletedBuilding();

            return building != null
                && building.BuildingTemplate.IsDestructible
                && IsWildPlant(building) == false;
        }

        public override void MarkForDestruction(Tile tile)
        {
            if (Covers(tile) == false)
                return;

            BuildingCmp building = tile.Entity.GetCompletedBuilding();

            if (GameplayScene.BuildingsRequiredDestructing)
                building.Destruct = true;
            else
                building.DestructBuilding();
        }

        private static bool IsWildPlant(BuildingCmp building)
        {
            return building is FarmPlot farmPlot && farmPlot.IsWild;
        }
    }
}
