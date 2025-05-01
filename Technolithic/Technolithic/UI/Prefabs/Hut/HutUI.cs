using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HutUI : MyPanelUI
    {

        public HutUI(Scene scene) : base(scene, Localization.GetLocalizedText("hut"), Color.White)
        {
            Width = 280;
            Height = 370;

            AddComponent(new HutUIScript());
        }

    }
}
