using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public class BuildingRangeRenderer
    {

        private HashSet<Tile> tiles = new HashSet<Tile>();

        private List<Tile> _tilesInRadius = new List<Tile>();

        public BuildingRangeRenderer()
        {

        }

        public void Update()
        {
            tiles.Clear();

            WorldManager worldManager = GameplayScene.WorldManager;

            Entity selectedEntity = worldManager.GetSelectedEntity();
            if (selectedEntity != null)
            {
                BuildingCmp buildingCmp = selectedEntity.Get<BuildingCmp>();

                if (buildingCmp == null)
                    return;

                if (buildingCmp.IsBuilt == false)
                    return;

                if (buildingCmp.BuildingTemplate.Range <= 0)
                    return;

                foreach (var tile in buildingCmp.RangeTiles)
                {
                    tiles.Add(tile);
                }
            }
            else
            {
                if (worldManager.CurrentAction != MyAction.Build)
                    return;

                if (worldManager.CurrentBuildingTemplate == null)
                    return;

                if (worldManager.CurrentBuildingTemplate.Range <= 0)
                    return;

                Tile buildingTemplateTile = worldManager.GetBoundedBuildingTemplateTile();
                var tilesCoveredByBuildingTemplate = worldManager.GetTilesCoveredByBuildingTemplate(buildingTemplateTile);

                foreach (var centerTile in tilesCoveredByBuildingTemplate)
                {
                    _tilesInRadius.Clear();
                    centerTile.World.TryGetTilesInRadius(centerTile.X, centerTile.Y, worldManager.CurrentBuildingTemplate.Range,
                        _tilesInRadius);

                    foreach (var checkTile in _tilesInRadius)
                    {
                        if (tilesCoveredByBuildingTemplate.Contains(checkTile))
                            continue;

                        tiles.Add(checkTile);
                    }
                }
            }
        }

        public void Render()
        {
            foreach (var tile in tiles)
            {
                RenderManager.Rect(tile.X * Engine.TILE_SIZE + 2, tile.Y * Engine.TILE_SIZE + 2,
                                Engine.TILE_SIZE - 4, Engine.TILE_SIZE - 4, Color.White * 0.5f);

                RenderManager.BorderRect(tile.X * Engine.TILE_SIZE + 2, tile.Y * Engine.TILE_SIZE + 2,
                                Engine.TILE_SIZE - 4, Engine.TILE_SIZE - 4, Color.Black * 0.5f);
            }
        }

    }
}
