using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MTextButtonUI : MNode
    {
        public ButtonScript ButtonScript { get; private set; }

        private MyText text;

        public MTextButtonUI(Scene scene) : base(scene)
        {
            ButtonScript = new ButtonScript();
            AddComponent(ButtonScript);

            MImageCmp image = new MImageCmp();
            image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            image.ImageType = ImageType.Sliced;
            image.SetBorder(8, 8, 8, 8);
            image.BackgroundOverlap = 2;
            AddComponent(image);

            ButtonScript.BackgroundImage = image;

            text = new MyText(scene);
            text.Name = "Text";
            text.Y = 8;
            AddChildNode(text);

            Height = 32;
            Width = 32;
        }

        public void CenterText()
        {
            int textWidth = text.TextWidth;
            int textHalfWidth = textWidth / 2;

            int buttonHalfWidth = Width / 2;

            text.X = buttonHalfWidth - textHalfWidth;
        }

    }
}
