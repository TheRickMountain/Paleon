using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class IlluminationOverlay : HighlightTileOverlay
    {
        public IlluminationOverlay(World world)
            : base(world,
                  GameOverlayType.Illumination,
                  ResourceManager.IlluminationOverlayIcon,
                  Localization.GetLocalizedText("illumination_overlay"),
                  Color.Yellow * 0.5f)
        {
        }

        protected override bool IsHighlighted(Tile tile)
        {
            return tile.IlluminationStrength > 0;
        }
    }
}
