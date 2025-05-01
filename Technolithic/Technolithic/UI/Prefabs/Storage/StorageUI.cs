using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class StorageUI : MyPanelUI
    {

        public StorageUI(Scene scene) : base(scene, Localization.GetLocalizedText("storage"), Color.White)
        {
            MyText spaceInfoText = new MyText(Scene);
            spaceInfoText.Height = 32;
            spaceInfoText.X = 8;
            spaceInfoText.Y = 8 + 32;
            spaceInfoText.Width = 200;
            spaceInfoText.Name = "SpaceInfo";
            AddChildNode(spaceInfoText);

            MSliderBar sliderBar = new MSliderBar(Scene, 320, 16);
            sliderBar.X = 8;
            sliderBar.Name = "SliderBar";
            sliderBar.Y = spaceInfoText.LocalY + spaceInfoText.Height + 5;
            AddChildNode(sliderBar);

            ListViewUI listView = new ListViewUI(scene, 350, 32, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = sliderBar.LocalY + sliderBar.Height + 5;

            Width = listView.Width + 16;
            Height = 400;

            AddChildNode(listView);
            AddComponent(new StorageUIScript());
        }

    }
}
