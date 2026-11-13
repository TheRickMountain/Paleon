using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class IrrigationOverlay : HighlightTileOverlay
    {
        public IrrigationOverlay(World world)
            : base(world,
                  GameOverlayType.Irrigation,
                  ResourceManager.IrrigationOverlayIcon,
                  Localization.GetLocalizedText("irrigation_overlay"),
                  Color.Blue * 0.5f)
        {
        }

        protected override bool IsHighlighted(Tile tile)
        {
            return tile.GroundTopType != GroundTopType.Water &&
                tile.GroundTopType != GroundTopType.DeepWater &&
                tile.GroundTopType != GroundTopType.IrrigationCanalFull &&
                tile.IrrigationLevel > 0;
        }
    }
}
