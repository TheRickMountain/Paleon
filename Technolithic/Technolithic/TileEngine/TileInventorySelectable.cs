

namespace Technolithic
{
    public class TileInventorySelectable : SelectableCmp
    {
        public Tile Tile { get; }
        public Inventory Inventory { get; }

        public TileInventorySelectable(Tile tile, Inventory inventory) 
            : base(tile.X * Engine.TILE_SIZE, tile.Y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, SelectableType.ItemContainers)
        {
            Tile = tile;
            Inventory = inventory;
        }
    }
}
