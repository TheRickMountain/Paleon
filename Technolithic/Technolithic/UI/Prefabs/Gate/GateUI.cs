using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class GateUI : MyPanelUI
    {

        public GateUI(Scene scene) : base(scene, "Gate", Color.White)
        {
            Width = 280;
            Height = 370;

            AddComponent(new GateUIScript());
        }

    }
}
