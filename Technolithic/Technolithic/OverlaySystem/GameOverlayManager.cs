namespace Technolithic
{
    public enum OverlayType
    {
        None,
        Irrigation,
        Illumination
    }

    public class GameOverlayManager : OverlayManager<OverlayType>
    {
        public GameOverlayManager(World world) 
            : base(world, OverlayType.None)
        {
            AddOverlay(OverlayType.Irrigation, new IrrigationOverlay());
            AddOverlay(OverlayType.Illumination, new IlluminationOverlay());
        }
    }
}
