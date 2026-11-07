using Microsoft.Xna.Framework;

namespace Technolithic
{
    public abstract class TileOverlay : GameOverlay
    {
        private readonly World _world;

        private readonly Color _highlightColor;

        protected TileOverlay(World world, GameOverlayType type, MyTexture icon, string name, Color highlightColor)
            : base(type, icon, name)
        {
            _world = world;
            _highlightColor = highlightColor;
        }

        protected abstract bool IsHighlighted(Tile tile);

        public override void Render()
        {
            Rectangle visibleTiles = _world.GetVisibleTileBounds();

            for (int x = visibleTiles.Left; x < visibleTiles.Right; x++)
            {
                for (int y = visibleTiles.Top; y < visibleTiles.Bottom; y++)
                {
                    if (IsHighlighted(_world.GetTileAt(x, y)))
                    {
                        RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, _highlightColor);
                    }
                }
            }
        }
    }
}
