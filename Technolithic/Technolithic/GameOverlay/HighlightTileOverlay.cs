using Microsoft.Xna.Framework;

namespace Technolithic
{
    public abstract class HighlightTileOverlay : TileOverlay
    {
        private readonly Color _highlightColor;

        protected HighlightTileOverlay(World world, GameOverlayType type, MyTexture icon, string name, Color highlightColor)
            : base(world, type, icon, name)
        {
            _highlightColor = highlightColor;
        }

        protected abstract bool IsHighlighted(Tile tile);

        protected override bool TryGetTileColor(Tile tile, out Color color)
        {
            color = _highlightColor;
            return IsHighlighted(tile);
        }
    }
}
