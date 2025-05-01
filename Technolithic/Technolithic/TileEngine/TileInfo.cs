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
        public bool IsWalkable { get; private set; }
        public bool IsTarget { get; private set; }
        public char GroundPattern { get; private set; }

        public TileInfo(bool walkable, bool target, char groundPattern)
        {
            IsWalkable = walkable;
            IsTarget = target;
            GroundPattern = groundPattern;
        }

    }
}
