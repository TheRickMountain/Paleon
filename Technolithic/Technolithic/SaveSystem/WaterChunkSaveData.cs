using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WaterChunkSaveData
    {
        public List<Tuple<int, int>> Tiles { get; set; }
        public int CurrentFishCount { get; set; }
    }
}
