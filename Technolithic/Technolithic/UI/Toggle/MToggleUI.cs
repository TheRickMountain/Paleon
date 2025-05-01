using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MToggleUI : MNode
    {

        public const int WIDTH = 32;
        public const int HEIGHT = 32;

        public MToggleUI(Scene scene, bool isOn, bool radio = false) : base(scene)
        {
            ToggleScript script = new ToggleScript(isOn, radio);
            AddComponent(script);
            Height = WIDTH;
            Width = HEIGHT;

            MNode background = new MNode(scene);
            MImageCmp backgroundImage = new MImageCmp();
            backgroundImage.Texture = RenderManager.Pixel;
            background.AddComponent(backgroundImage);
            background.Width = WIDTH;
            background.Height = HEIGHT;

            AddChildNode(background);

            script.BackgroundImage = backgroundImage;
        }

    }
}
