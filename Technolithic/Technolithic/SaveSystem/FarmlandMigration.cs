using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    // Crops used to be sown on any ground; now sowing needs farmland. Tiles under the player's crops and
    // sowing sites become farmland so that the sites are not dropped on load. Wild plants keep their ground.
    // Other buildings may not stand on farmland, so farmland under them becomes plain ground.
    public static class FarmlandMigration
    {
        public static void Apply(SaveData data, Func<string, bool> isCrop, Func<string, bool> isAllowedOnFarmland)
        {
            foreach (BuildingSaveData building in data.BuildingSaveDatas)
            {
                if (building.IsWild == false && isCrop(building.BuildingTemplateName))
                    PlowUnder(data, building);
                else if (isAllowedOnFarmland(building.BuildingTemplateName) == false)
                    ClearFarmlandUnder(data, building);
            }
        }

        private static void PlowUnder(SaveData data, BuildingSaveData building)
        {
            foreach (Point position in building.Tiles)
            {
                TileSaveData tile = data.WorldSaveData.Tiles[position.X, position.Y];

                if (Tile.IsFarmlandGround((GroundType)tile.GroundType) == false)
                    tile.GroundType = (int)GroundType.FarmPlot;
            }
        }

        private static void ClearFarmlandUnder(SaveData data, BuildingSaveData building)
        {
            foreach (Point position in building.Tiles)
            {
                TileSaveData tile = data.WorldSaveData.Tiles[position.X, position.Y];

                if (Tile.IsFarmlandGround((GroundType)tile.GroundType))
                {
                    tile.GroundType = (int)GroundType.Ground;
                    tile.IrrigateFarmland = false;
                    tile.FertilizeFarmland = false;
                }
            }
        }
    }
}
