using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class MovementSpeedOverlay : TileOverlay
    {
        private const int SlowestSpeedPercent = 30;
        private const int FastestSpeedPercent = 100;

        public MovementSpeedOverlay(World world)
            : base(world,
                  GameOverlayType.MovementSpeed,
                  ResourceManager.MovementSpeedOverlayIcon,
                  Localization.GetLocalizedText("movement_speed_overlay"))
        {
        }

        protected override bool TryGetTileColor(Tile tile, out Color color)
        {
            color = GetSpeedColor(tile.MovementSpeedPercent);
            return tile.IsWalkable;
        }

        public static Color GetSpeedColor(int movementSpeedPercent)
        {
            float speed = MathHelper.Clamp(
                (movementSpeedPercent - SlowestSpeedPercent) / (float)(FastestSpeedPercent - SlowestSpeedPercent), 0, 1);

            Color color = speed < 0.5f
                ? Color.Lerp(Color.Red, Color.Yellow, speed * 2)
                : Color.Lerp(Color.Yellow, Color.Lime, (speed - 0.5f) * 2);

            return color * 0.5f;
        }
    }
}
