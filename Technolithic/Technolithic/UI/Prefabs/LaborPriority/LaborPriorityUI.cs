using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class LaborPriorityUI : MyPanelUI
    {

        public LaborPriorityUI(Scene scene) : base(scene, null, Color.White)
        {
            int laborCount = Labor.GetWorkLaborEnumerator().Count();

            ListViewUI listView = new ListViewUI(scene, 280 + laborCount * 32 + (laborCount - 1) * 16, 48, 6);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 80;

            Width = listView.Width + 16;
            Height = listView.Height + 96;

            AddChildNode(listView);
            AddComponent(new LaborPriorityUIScript());
        }

    }
}
