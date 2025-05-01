using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TilePath
    {

        public List<Tile> Tiles { get; private set; }
        public Tile TargetTile { get; private set; }
        public bool Adjacent { get; private set; }


        public TilePath(List<Tile> tiles, Tile targetTile, bool adjacent)
        {
            Tiles = tiles;
            TargetTile = targetTile;
            Adjacent = adjacent;
        }

        public int Count
        {
            get { return Tiles.Count; }
        }

        public void RemoveAt(int index)
        {
            Tiles.RemoveAt(index);
        }

        public Tile this[int index]
        {
            get { return Tiles[index]; }
        }

    }
}
