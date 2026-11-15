namespace Technolithic
{
    // Marks a single tile layer for removal, or cancels the mark, e.g. from the layer's panel.
    public static class TileLayerDestruction
    {
        public static bool CanMark(Tile tile, ITileLayer layer)
        {
            return layer.GetTemplate(tile)?.IsDestructible == true && DestructFilters.ForLayer(layer) != null;
        }

        public static bool IsMarked(Tile tile, ITileLayer layer)
        {
            return GetRemovalSite(tile, layer) != null;
        }

        public static void Toggle(Tile tile, ITileLayer layer)
        {
            ConstructionSiteCmp removalSite = GetRemovalSite(tile, layer);

            if (removalSite != null)
                removalSite.Cancel();
            else if (CanMark(tile, layer))
                DestructFilters.ForLayer(layer).MarkForDestruction(tile);
        }

        private static ConstructionSiteCmp GetRemovalSite(Tile tile, ITileLayer layer)
        {
            ConstructionSiteCmp site = tile.Entity.GetConstructionSite();

            return site?.BuildingTemplate.TileChange is RemoveTileLayer removal && removal.Layer == layer ? site : null;
        }
    }
}
