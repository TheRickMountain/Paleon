using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalPenUI : MyPanelUI
    {

        public AnimalPenUI(Scene scene) : base(scene, Localization.GetLocalizedText("animal_filter"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 320, 32, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 32;

            Width = listView.Width + 16;
            Height = listView.Height + 40;

            AddChildNode(listView);
            AddComponent(new AnimalPenUIScript());
        }

    }
}