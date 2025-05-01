using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PriorityUI : MyPanelUI
    {

        public PriorityUI(Scene scene) : base(scene, Localization.GetLocalizedText("priority"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 250, 32, 9);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            Width = listView.Width + 16;
            Height = listView.Height + 48;

            AddChildNode(listView);
            AddComponent(new PriorityUIScript());
        }

    }
}
