using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class VeryBigButton : MButtonUI
    {
        public VeryBigButton(Scene scene, MyTexture iconTexture, int iconWidth, int iconHeight, bool IsSelectable) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(32, 64, 40, 40);

            Width = 80;
            Height = 80;

            if (iconHeight > Height || iconWidth > Width)
            {
                iconHeight = (int)(iconHeight * 0.5f);
                iconWidth = (int)(iconWidth * 0.5f);
            }

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = iconTexture;
            icon.X = (Width - iconWidth) / 2;
            icon.Y = (Height - iconHeight) / 2;
            icon.Width = iconWidth;
            icon.Height = iconHeight;

            AddChildNode(icon);

            ButtonScript.IsSelectable = IsSelectable;
        }
    }
}
