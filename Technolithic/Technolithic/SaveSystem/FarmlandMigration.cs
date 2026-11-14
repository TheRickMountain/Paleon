using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    // Crops used to be sown on any ground; now sowing needs farmland. Tiles under the player's crops and
    // sowing sites become farmland so that the sites are not dropped on load. Wild plants keep their ground.
    public static class FarmlandMigration
    {
        public static void Apply(SaveData data, Func<string, bool> isCrop)
        {
            foreach (BuildingSaveData building in data.BuildingSaveDatas)
            {
                if (building.IsWild || isCrop(building.BuildingTemplateName) == false)
                    continue;

                foreach (Point position in building.Tiles)
                {
                    TileSaveData tile = data.WorldSaveData.Tiles[position.X, position.Y];

                    if (Tile.IsFarmlandGround((GroundType)tile.GroundType) == false)
                        tile.GroundType = (int)GroundType.FarmPlot;
                }
            }
        }
    }
}
