using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LightEmitter
    {

        public int Radius { get; private set; }

        public LightEmitter(int radius)
        {
            Radius = radius;
        }

    }
}
