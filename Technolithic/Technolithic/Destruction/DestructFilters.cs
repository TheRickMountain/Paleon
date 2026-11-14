using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public static class DestructFilters
    {
        // Order matters: a layer can only be removed from a free tile, and a canal only from under no bridge.
        public static IReadOnlyList<DestructFilter> All { get; } = new DestructFilter[]
        {
            new BuildingDestructFilter("buildings", "buildings", () => TextureBank.UITexture.GetSubtexture(208, 80, 16, 16), true),
            new TileLayerDestructFilter("walls", "walls", () => TextureBank.UITexture.GetSubtexture(144, 80, 16, 16),
                true, TileLayers.Wall, "destruct_wall"),
            new TileLayerDestructFilter("surfaces", "floor", () => TextureBank.UITexture.GetSubtexture(240, 80, 16, 16),
                false, TileLayers.Surface, "destruct_surface"),
            new TileLayerDestructFilter("canals", "irrigation_canals", () => ResourceManager.GetTexture("irrigation_canal"),
                false, TileLayers.IrrigationCanal, "destruct_irrigation_canal",
                progressTree => progressTree.IsBuildingUnlocked(Engine.Instance.Buildings["build_irrigation_canal"])),
            new TileLayerDestructFilter("farmland", "farmland", () => ResourceManager.GetTexture("farmland"),
                false, TileLayers.Farmland, "destruct_farmland",
                progressTree => progressTree.IsBuildingUnlocked(Engine.Instance.Buildings["farmland"]))
        };

        private static readonly HashSet<DestructFilter> enabledFilters = new HashSet<DestructFilter>(
            All.Where(filter => filter.IsEnabledByDefault));

        public static IReadOnlyList<DestructFilter> GetAvailable(ProgressTree progressTree)
        {
            return All.Where(filter => filter.IsAvailable(progressTree)).ToList();
        }

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

        public static void MarkForDestruction(Tile[,] tiles, ProgressTree progressTree)
        {
            IReadOnlyList<DestructFilter> activeFilters = GetAvailable(progressTree).Where(IsEnabled).ToList();

            foreach (Tile tile in tiles)
            {
                foreach (DestructFilter filter in activeFilters)
                {
                    filter.MarkForDestruction(tile);
                }
            }
        }
    }
}
