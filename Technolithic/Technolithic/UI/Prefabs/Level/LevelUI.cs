using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class LevelUI : MButtonUI
    {

        public LevelUI(Scene scene) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            Image.ImageType = ImageType.Sliced;
            Image.SetBorder(8, 8, 8, 8);
            Image.BackgroundOverlap = 2;

            Width = 260;
            Height = 72;

            AddComponent(new LevelUIScript());
        }

    }
}
