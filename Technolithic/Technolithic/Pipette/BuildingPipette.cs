using System.Collections.Generic;

namespace Technolithic
{
    // Takes what the player points at as the next thing to build, like an eyedropper:
    // the building or construction site on the tile, otherwise its topmost tile layer.
    public static class BuildingPipette
    {
        public static bool TryPick(Tile tile, ProgressTree progressTree)
        {
            foreach (var (template, direction) in GetCandidates(tile))
            {
                if (CanPick(template, progressTree))
                {
                    Pick(template, direction);
                    return true;
                }
            }

            return false;
        }

        public static bool CanPick(BuildingTemplate template, ProgressTree progressTree)
        {
            return template.Cloneable && progressTree.IsBuildingUnlocked(template);
        }

        public static void Pick(BuildingTemplate template, Direction direction)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.Build, null);
            GameplayScene.WorldManager.SetBuilding(template, direction);
            GameplayScene.UIRootNodeScript.CloseEntityPanel();
        }

        private static IEnumerable<(BuildingTemplate Template, Direction Direction)> GetCandidates(Tile tile)
        {
            BuildingPlacement placement = tile.Entity.GetBuildingPlacement();
            if (placement != null)
                yield return (placement.Template, placement.Direction);

            foreach (ITileLayer layer in TileLayers.All)
            {
                BuildingTemplate template = layer.GetTemplate(tile);
                if (template != null)
                    yield return (template, Direction.DOWN);
            }
        }
    }
}
