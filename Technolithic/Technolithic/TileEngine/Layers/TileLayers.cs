using System.Collections.Generic;

namespace Technolithic
{
    public static class TileLayers
    {
        public static ITileLayer Surface { get; } = new SurfaceLayer();
        public static ITileLayer IrrigationCanal { get; } = new IrrigationCanalLayer();
        public static ITileLayer Wall { get; } = new WallLayer();
        public static ITileLayer Farmland { get; } = new FarmlandLayer();

        public static IReadOnlyList<ITileLayer> All { get; } = new[] { Surface, IrrigationCanal, Wall, Farmland };
    }
}
