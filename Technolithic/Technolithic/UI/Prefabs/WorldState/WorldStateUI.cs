using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class WorldStateUI : MyPanelUI
    {

        public WorldStateUI(Scene scene) : base(scene, null, Color.White)
        {
            ClipContent = true;
            int imageSize = 96;

            MImageUI sunAndMoonImage = new MImageUI(scene);
            sunAndMoonImage.Name = "SunAndMoon";
            sunAndMoonImage.GetComponent<MImageCmp>().Texture = ResourceManager.SunAndMoonSprite;
            sunAndMoonImage.GetComponent<MImageCmp>().CenterOrigin();
            sunAndMoonImage.X = 8 + imageSize / 2;
            sunAndMoonImage.Y = 8 + imageSize / 2 + 8;
            sunAndMoonImage.Width = imageSize;
            sunAndMoonImage.Height = imageSize;
            AddChildNode(sunAndMoonImage);

            MImageUI pointerImage = new MImageUI(scene);
            pointerImage.GetComponent<MImageCmp>().Texture = ResourceManager.SunAndMoonPointerSprite;
            pointerImage.GetComponent<MImageCmp>().CenterOrigin();
            pointerImage.X = 8 + imageSize / 2;
            pointerImage.Y = 8 + imageSize / 2;
            pointerImage.Width = imageSize;
            pointerImage.Height = imageSize;
            AddChildNode(pointerImage);

            MyText state = new MyText(scene);
            state.Text = "";
            state.Name = "State";
            state.X = 8 + imageSize + 5;
            state.Y = 8;
            state.Width = 200;
            state.Height = 56;
            AddChildNode(state);

            MyText settlers = new MyText(scene);
            settlers.Text = "Settlers: 4";
            settlers.Name = "Settlers";
            settlers.X = 8 + state.Width;
            settlers.Y = 8;
            settlers.Width = 0;
            settlers.Height = 0;
            AddChildNode(settlers);

            MyText food = new MyText(scene);
            food.Text = "Food: 10";
            food.Name = "Food";
            food.X = 8 + settlers.LocalX + 150;
            food.Y = 8;
            food.Width = 0;
            food.Height = 0;
            AddChildNode(food);

            MyText wind = new MyText(scene);
            wind.Text = "Wind: Calm";
            wind.Name = "Wind";
            wind.X = 8 + food.LocalX + 100;
            wind.Y = 8;
            wind.Width = 0;
            wind.Height = 0;
            AddChildNode(wind);

            Height = 70;
            Width = 660;

            AddComponent(new WorldStateUIScript());
        }

    }
}
