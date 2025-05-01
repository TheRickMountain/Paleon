using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LineUI : MNode
    {

        public LineUI(Scene scene, Vector2 start, Vector2 end, Color color, float thickness, bool outlined = false) : base(scene)
        {
            AddComponent(new MLineCmp(start, end, color, thickness, outlined));
        }

    }
}
