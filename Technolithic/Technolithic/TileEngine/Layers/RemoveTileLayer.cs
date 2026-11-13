namespace Technolithic
{
    public class RemoveTileLayer : ITileChange
    {
        private readonly ITileLayer layer;

        public RemoveTileLayer(ITileLayer layer)
        {
            this.layer = layer;
        }

        public void Apply(Tile tile)
        {
            BuildingTemplate removedTemplate = layer.GetTemplate(tile);

            if (removedTemplate != null)
            {
                foreach (var kvp in removedTemplate.BuildingRecipe)
                {
                    Item item = kvp.Key;
                    int weight = kvp.Value;

                    if (weight > 0)
                    {
                        tile.Inventory.AddCargo(new ItemContainer(item, weight, item.Durability));
                    }
                }
            }

            layer.Remove(tile);
        }
    }
}
