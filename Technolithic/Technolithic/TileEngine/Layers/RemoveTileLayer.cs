namespace Technolithic
{
    public class RemoveTileLayer : ITileChange
    {
        private readonly ITileLayer layer;

        public ITileLayer Layer => layer;

        public RemoveTileLayer(ITileLayer layer)
        {
            this.layer = layer;
        }

        public void Apply(Tile tile)
        {
            BuildingTemplate removedTemplate = layer.GetTemplate(tile);

            // Removed before the refund: a wall tile has no room yet, so haulers would never find items dropped on it
            layer.Remove(tile);

            if (removedTemplate != null)
            {
                foreach (var kvp in removedTemplate.BuildingRecipe)
                {
                    Item item = kvp.Key;
                    int weight = kvp.Value;

                    if (weight > 0)
                    {
                        tile.World.TileItems.Drop(tile, new ItemContainer(item, weight, item.Durability));
                    }
                }
            }
        }
    }
}
