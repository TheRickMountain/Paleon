using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AchievementsUI : MyPanelUI
    {

        public AchievementsUI(Scene scene) : base(scene, Localization.GetLocalizedText("achievements"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 400, 48, 9);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 64;

            Width = listView.Width + 16;
            Height = listView.Height + 88;

            AddChildNode(listView);

            AddComponent(new AchievementsUIScript());
        }

    }
}
