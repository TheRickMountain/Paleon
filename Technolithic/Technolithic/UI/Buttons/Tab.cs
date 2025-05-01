using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Tab : MButtonUI
    {
        public Tab(Scene scene, MyTexture iconTexture, bool IsSelectable) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(176, 160, 24, 24);

            Width = 48;
            Height = 48;

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = iconTexture;
            icon.X = 8;
            icon.Y = 8;
            icon.Width = 32;
            icon.Height = 32;
            icon.Name = "Icon";

            AddChildNode(icon);

            ButtonScript.IsSelectable = IsSelectable;
        }

    }
}
