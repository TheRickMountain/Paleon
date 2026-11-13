namespace Technolithic
{
    public class PlaceTileLayer : ITileChange
    {
        private readonly ITileLayer layer;
        private readonly BuildingTemplate template;

        public PlaceTileLayer(ITileLayer layer, BuildingTemplate template)
        {
            this.layer = layer;
            this.template = template;
        }

        public void Apply(Tile tile)
        {
            layer.Place(tile, template);
        }
    }
}
