using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Slot : MButtonUI
    {
        public Slot(Scene scene) : base(scene)
        {
            Image.Texture = ResourceManager.Slot24;

            Width = 48;
            Height = 48;

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = RenderManager.Pixel;
            icon.X = 8;
            icon.Y = 8;
            icon.Width = 32;
            icon.Height = 32;
            icon.Name = "Icon";
            icon.Active = false;

            AddChildNode(icon);
        }

    }
}
