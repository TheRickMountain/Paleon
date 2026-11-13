using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    // Finished walls used to be saved as buildings; now they are a tile layer (TileSaveData.WallId).
    // Wall construction sites stay buildings. Running it on an already converted save changes nothing.
    public static class WallsToTileLayerMigration
    {
        public const string DestructWallTemplateName = "destruct_wall";

        public static void Apply(SaveData data, Func<string, int?> getWallId)
        {
            List<BuildingSaveData> buildings = data.BuildingSaveDatas;
            TileSaveData[,] tiles = data.WorldSaveData.Tiles;

            for (int i = buildings.Count - 1; i >= 0; i--)
            {
                BuildingSaveData building = buildings[i];
                int? wallId = getWallId(building.BuildingTemplateName);

                if (wallId == null || building.IsBuilt == false)
                    continue;

                Point position = building.Tiles[0, 0];
                tiles[position.X, position.Y].WallId = wallId.Value;

                if (building.Destruct)
                    buildings[i] = CreateDestructWallSite(building);
                else
                    buildings.RemoveAt(i);
            }
        }

        private static BuildingSaveData CreateDestructWallSite(BuildingSaveData wall)
        {
            return new BuildingSaveData
            {
                BuildingTemplateName = DestructWallTemplateName,
                Tiles = wall.Tiles,
                Direction = wall.Direction,
                IsBuilt = false
            };
        }
    }
}
