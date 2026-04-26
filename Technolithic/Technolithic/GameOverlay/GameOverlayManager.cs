using System.Collections.Generic;

namespace Technolithic
{
    public enum GameOverlayType
    {
        None,
        Irrigation,
        Illumination
    }

    public class GameOverlayManager : OverlayManager<GameOverlayType>
    {
        private List<GameOverlay> _overlays;

        public IReadOnlyList<GameOverlay> Overlays => _overlays;

        public GameOverlayManager(World world) 
            : base(world, GameOverlayType.None)
        {
            _overlays = new List<GameOverlay>();

            AddOverlay(new IrrigationOverlay());
            AddOverlay(new IlluminationOverlay());
        }

        private void AddOverlay(GameOverlay gameOverlay)
        {
            AddOverlay(gameOverlay.Type, gameOverlay);

            _overlays.Add(gameOverlay);
        }
    }
}
