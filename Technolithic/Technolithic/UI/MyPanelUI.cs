using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MyPanelUI : MNode
    {

        public MImageCmp ImageUI;
        public RichTextUI LabelUI;

        public MyPanelUI(Scene scene, string name, Color color) : base(scene)
        {
            ImageUI = new MImageCmp();
            ImageUI.Texture = TextureBank.UITexture.GetSubtexture(112, 64, 24, 24);
            ImageUI.ImageType = ImageType.Sliced;
            ImageUI.SetBorder(8, 8, 8, 8);
            ImageUI.Color = color;

            AddComponent(ImageUI);

            X = 400;
            Y = 400;
            Width = 250;
            Height = 400;

            if (name != null)
            {
                LabelUI = new RichTextUI(scene);
                LabelUI.Text = name;
                LabelUI.Color = Color.LightBlue;
                LabelUI.Name = "Label";
                LabelUI.X = 8;
                LabelUI.Y = 8;

                AddChildNode(LabelUI);
            }
        }

    }
}
