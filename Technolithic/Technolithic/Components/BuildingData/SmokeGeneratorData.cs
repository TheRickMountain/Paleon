using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SmokeGeneratorData
    {

        public Vector2 SpawnPosition { get; private set; }

        public SmokeGeneratorData(float x, float y)
        {
            SpawnPosition = new Vector2(x, y);
        }

    }
}
