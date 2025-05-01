using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AssignHutUI : MyPanelUI
    {

        public AssignHutUI(Scene scene) : base(scene, Localization.GetLocalizedText("settlers"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 300, 48, 6);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            AddChildNode(listView);

            Width = listView.Width + 16;
            Height = listView.Height + 96;

            AddComponent(new AssignHutUIScript());
        }

    }
}
