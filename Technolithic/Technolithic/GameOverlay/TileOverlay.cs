using Microsoft.Xna.Framework;

namespace Technolithic
{
    public abstract class TileOverlay : GameOverlay
    {
        private readonly World _world;

        protected TileOverlay(World world, GameOverlayType type, MyTexture icon, string name)
            : base(type, icon, name)
        {
            _world = world;
        }

        protected abstract bool TryGetTileColor(Tile tile, out Color color);

        public override void Render()
        {
            Rectangle visibleTiles = _world.GetVisibleTileBounds();

            for (int x = visibleTiles.Left; x < visibleTiles.Right; x++)
            {
                for (int y = visibleTiles.Top; y < visibleTiles.Bottom; y++)
                {
                    if (TryGetTileColor(_world.GetTileAt(x, y), out Color color))
                    {
                        RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, color);
                    }
                }
            }
        }
    }
}
