using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class TechTreeUI : MyPanelUI
    {

        public TechTreeUI(Scene scene) : base(scene, Localization.GetLocalizedText("technologies"), Color.White)
        {
            Width = Engine.Width;
            Height = Engine.Height;

            int elementsCount = (Engine.Height - 40) / (48 + 5) - 1;

            ListViewUI listView = new ListViewUI(scene, 360, 48, elementsCount);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 40;
            AddChildNode(listView);

            AddComponent(new TechTreeUIScript());
        }

    }
}
