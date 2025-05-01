using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldSaveData
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public TileSaveData[,] Tiles { get; set; }

    }
}
