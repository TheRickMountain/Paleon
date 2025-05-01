using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ItemsStackUI : MyPanelUI
    {

        public ItemsStackUI(Scene scene) : base(scene, Localization.GetLocalizedText("items"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 250, 32, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            Width = listView.Width + 16;
            Height = 370;

            AddChildNode(listView);
            AddComponent(new ItemsStackUIScript());
        }

    }
}
