using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MImageUI : MNode
    {

        public MImageCmp Image { get; private set; }

        public MImageUI(Scene scene) : base(scene)
        {
            Image = new MImageCmp();
            AddComponent(Image);
        }

    }
}
