using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class TradingUI : MyPanelUI
    {

        public TradingUI(Scene scene) : base(scene, Localization.GetLocalizedText("trading"), Color.White)
        {
            Width = 312 * 3 + (5 * 2) + 16;
            Height = 550;

            AddComponent(new TradingUIScript());
        }

    }
}