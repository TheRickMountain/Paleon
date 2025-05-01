using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldSettingsUI : MyPanelUI
    {

        public WorldSettingsUI(Scene scene, string title) : base(scene, title, Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 300, 40, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            AddChildNode(listView);

            Width = listView.Width + 16;
            Height = 450;

            AddComponent(new WorldSettingsUIScript());
        }

    }
}
