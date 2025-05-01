using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BigButton : MButtonUI
    {
        public BigButton(Scene scene, MyTexture iconTexture, bool IsSelectable, bool hasExclamationMark = false) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(0, 64, 24, 24);

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

            if(hasExclamationMark)
            {
                MImageUI exclamationMark = new MImageUI(scene);
                exclamationMark.X = 16;
                exclamationMark.Y = 16;
                exclamationMark.Width = 32;
                exclamationMark.Height = 32;
                exclamationMark.GetComponent<MImageCmp>().Texture = ResourceManager.ExclamationMarkIcon;
                exclamationMark.Name = "ExclamationMark";
                exclamationMark.Active = false;
                AddChildNode(exclamationMark);
            }

            ButtonScript.IsSelectable = IsSelectable;
        }

    }
}
