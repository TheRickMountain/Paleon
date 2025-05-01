using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class NomadsUI : MyPanelUI
    {

        public NomadsUI(Scene scene) : base(scene, Localization.GetLocalizedText("new_settlers_arrived"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 300, 48, 6);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            AddChildNode(listView);

            Width = listView.Width + 16;
            Height = listView.Height + 96;

            AddComponent(new NomadsUIScript());
        }

    }
}
