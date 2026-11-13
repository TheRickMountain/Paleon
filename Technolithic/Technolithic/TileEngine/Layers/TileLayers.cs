namespace Technolithic
{
    public static class TileLayers
    {
        public static ITileLayer Surface { get; } = new SurfaceLayer();
        public static ITileLayer IrrigationCanal { get; } = new IrrigationCanalLayer();
    }
}
