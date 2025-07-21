using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TileSaveData
    {
        public int GroundType { get; set; }
        public int GroundTopType { get; set; }
        public int SurfaceId { get; set; }
        public int WallId { get; set; }
        public List<Tuple<int, int, float>> InventoryItems { get; set; }
        public float MoistureLevel { get; set; }
        public float FertilizerLevel { get; set; }
        public int IrrigationStrength { get; set; }
    }
}
