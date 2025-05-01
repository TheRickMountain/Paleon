using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MButtonUI : MNode
    {

        public ButtonScript ButtonScript { get; private set; }
        public MImageCmp Image { get; private set; }

        public MButtonUI(Scene scene) : base(scene)
        {
            ButtonScript = new ButtonScript();
            AddComponent(ButtonScript);

            Image = new MImageCmp();
            Image.Texture = RenderManager.Pixel;
            AddComponent(Image);

            ButtonScript.BackgroundImage = Image;

            Height = 32;
            Width = 32;
        }

    }
}
