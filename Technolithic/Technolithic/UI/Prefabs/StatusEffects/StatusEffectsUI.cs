using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Technolithic
{
    public class StatusEffectsUI : MyPanelUI
    {
        public StatusEffectsUI(Scene scene) : base(scene, Localization.GetLocalizedText("effects"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 250, 28, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;
            Width = listView.Width + 16;
            Height = 370;
            AddChildNode(listView);
            AddComponent(new StatusEffectsUIScript());
        }
    }
}