using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SmallButton : MButtonUI
    {

        public SmallButton(Scene scene, MyTexture iconTexture) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(16, 48, 16, 16);

            Width = 32;
            Height = 32;

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = iconTexture;
            icon.Width = 32;
            icon.Height = 32;
            icon.Name = "Icon";

            AddChildNode(icon);
        }

    }
}
