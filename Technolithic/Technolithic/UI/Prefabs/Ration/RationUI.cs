using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class RationUI : MyPanelUI
    {

        public RationUI(Scene scene) : base(scene, Localization.GetLocalizedText("ration"), Color.White)
        {
            int foodAmount = Engine.Instance.SettlerRation.Count;

            ListViewUI listView = new ListViewUI(scene, 250 + foodAmount * 32 + (foodAmount - 1) * 16, 48, 6);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 96;

            Width = listView.Width + 16;
            Height = listView.Height + 16 + 96;

            AddChildNode(listView);
            AddComponent(new RationUIScript());
        }

    }
}
