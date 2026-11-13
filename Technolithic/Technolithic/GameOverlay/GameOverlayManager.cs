using System.Collections.Generic;

namespace Technolithic
{
    public enum GameOverlayType
    {
        None,
        Irrigation,
        Illumination,
        MovementSpeed
    }

    public class GameOverlayManager : OverlayManager<GameOverlayType>
    {
        private List<GameOverlay> _overlays;

        public IReadOnlyList<GameOverlay> Overlays => _overlays;

        public GameOverlayManager(World world) 
            : base(GameOverlayType.None)
        {
            _overlays = new List<GameOverlay>();

            RegisterOverlay(new IrrigationOverlay(world));
            RegisterOverlay(new IlluminationOverlay(world));
            RegisterOverlay(new MovementSpeedOverlay(world));
        }

        private void RegisterOverlay(GameOverlay gameOverlay)
        {
            RegisterOverlay(gameOverlay.Type, gameOverlay);

            _overlays.Add(gameOverlay);
        }
    }
}
