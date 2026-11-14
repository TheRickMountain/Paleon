using System.Collections.Generic;

namespace Technolithic
{
    public static class TileLayers
    {
        public static ITileLayer Surface { get; } = new SurfaceLayer();
        public static ITileLayer IrrigationCanal { get; } = new IrrigationCanalLayer();
        public static ITileLayer Wall { get; } = new WallLayer();
        public static ITileLayer Farmland { get; } = new FarmlandLayer();

        // Top to bottom, so the first present layer is the one the player sees.
        public static IReadOnlyList<ITileLayer> All { get; } = new[] { Wall, Surface, IrrigationCanal, Farmland };
    }
}
