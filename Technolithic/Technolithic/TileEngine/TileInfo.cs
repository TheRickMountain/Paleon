using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TileInfo
    {
        public Tile Tile { get; set; }
        public bool IsTarget { get; private set; }
        public char GroundPattern { get; private set; }

        public TileInfo(bool target, char groundPattern)
        {
            IsTarget = target;
            GroundPattern = groundPattern;
        }

    }
}
