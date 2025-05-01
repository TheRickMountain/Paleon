using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Technolithic
{
    public class TimeControllerUI : MNode
    {
        public TimeControllerUI(Scene scene) : base(scene)
        {
            SmallButton first = new SmallButton(scene, TextureBank.UITexture.GetSubtexture(64, 32, 16, 16));
            first.Name = "First";
            AddChildNode(first);

            SmallButton second = new SmallButton(scene, TextureBank.UITexture.GetSubtexture(80, 32, 16, 16));
            second.X = first.LocalX + first.Width + 5;
            second.Name = "Second";
            AddChildNode(second);

            SmallButton fourth = new SmallButton(scene, TextureBank.UITexture.GetSubtexture(96, 32, 16, 16));
            fourth.X = second.LocalX + second.Width + 5;
            fourth.Name = "Fourth";
            AddChildNode(fourth);

            SmallButton pause = new SmallButton(scene, TextureBank.UITexture.GetSubtexture(112, 32, 16, 16));
            pause.X = fourth.LocalX + fourth.Width + 5;
            pause.Name = "Pause";
            AddChildNode(pause);

            Width = first.LocalX + pause.LocalX + pause.Width;
            Height = first.Height;

            AddComponent(new TimeControllerUIScript());
        }

    }
}
