using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Fish
    {

        public Vector2 Position { get; set; }
        public  Tile CurrentTile { get; set; }
        public Tile TargetTile { get; set; }
        public float RestTime { get; set; } = 0;
        public float Rotation { get; set; } = 0;

        public float MovementProgress { get; set; } = 0;

        public Fish(Vector2 position)
        {
            Position = position;
        }

    }
}
