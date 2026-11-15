namespace Technolithic
{
    public class TileLayerSelectable : SelectableCmp
    {
        public Tile Tile { get; }
        public ITileLayer Layer { get; }

        public TileLayerSelectable(Tile tile, ITileLayer layer)
            : base(tile.X * Engine.TILE_SIZE, tile.Y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, SelectableType.TileLayer)
        {
            Tile = tile;
            Layer = layer;
        }
    }
}
