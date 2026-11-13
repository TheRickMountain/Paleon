using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public static class DestructFilters
    {
        // Buildings come first: a wall or a surface can only be removed from a free tile.
        public static IReadOnlyList<DestructFilter> All { get; } = new DestructFilter[]
        {
            new BuildingDestructFilter("buildings", "buildings", () => TextureBank.UITexture.GetSubtexture(208, 80, 16, 16), true),
            new TileLayerDestructFilter("walls", "walls", () => TextureBank.UITexture.GetSubtexture(144, 80, 16, 16),
                true, TileLayers.Wall, "destruct_wall"),
            new TileLayerDestructFilter("surfaces", "floor", () => TextureBank.UITexture.GetSubtexture(240, 80, 16, 16),
                false, TileLayers.Surface, "destruct_surface")
        };

        private static readonly HashSet<DestructFilter> enabledFilters = new HashSet<DestructFilter>(
            All.Where(filter => filter.IsEnabledByDefault));

        public static bool IsEnabled(DestructFilter filter)
        {
            return enabledFilters.Contains(filter);
        }

        public static void SetEnabled(DestructFilter filter, bool isEnabled)
        {
            if (isEnabled)
                enabledFilters.Add(filter);
            else
                enabledFilters.Remove(filter);
        }

        public static void Toggle(DestructFilter filter)
        {
            SetEnabled(filter, IsEnabled(filter) == false);
        }

        public static void EnableOnly(DestructFilter filter)
        {
            enabledFilters.Clear();
            enabledFilters.Add(filter);
        }

        public static void EnableAll()
        {
            enabledFilters.UnionWith(All);
        }

        public static void MarkForDestruction(Tile[,] tiles)
        {
            foreach (Tile tile in tiles)
            {
                foreach (DestructFilter filter in All.Where(IsEnabled))
                {
                    filter.MarkForDestruction(tile);
                }
            }
        }
    }
}
