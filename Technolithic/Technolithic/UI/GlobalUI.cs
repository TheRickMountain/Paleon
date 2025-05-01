using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GlobalUI
    {

        private static MSliderBar actionProgressSlider;
        private static RichTextUI tooltipsText;
        private static MImage tooltipsBackground;

        public static void Initialize()
        {
            actionProgressSlider = new MSliderBar(null, 100, 16);
            actionProgressSlider.CheckInput = false;
            actionProgressSlider.ShowDragger = false;
            actionProgressSlider.Active = false;
            actionProgressSlider.MaxValue = 100;
            actionProgressSlider.CurrentValue = 0;

            tooltipsText = new RichTextUI(null);
            tooltipsText.Active = false;

            tooltipsBackground = new MImage(null);
            tooltipsBackground.Active = false;
            tooltipsBackground.Texture = RenderManager.Pixel;
            tooltipsBackground.Color = Color.Black * 0.9f;
        }

        public static void Update()
        {
            actionProgressSlider.Active = false;
            tooltipsText.Active = false;
            tooltipsBackground.Active = false;
        }

        public static void Render()
        {
            actionProgressSlider.Render();
            tooltipsBackground.Render();
            tooltipsText.Render();
        }

        public static void ShowActionProgressSlider(int x, int y, Color color, int maxValue, int currentValue)
        {
            actionProgressSlider.X = x;
            actionProgressSlider.Y = y;
            actionProgressSlider.FrontColor = color;
            actionProgressSlider.MaxValue = maxValue;
            actionProgressSlider.CurrentValue = currentValue;
            actionProgressSlider.Active = true;
        }

        public static void ShowTooltips(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                text = "???";
            }

            tooltipsText.Active = true;
            tooltipsBackground.Active = true;

            tooltipsText.Text = text;

            int posX = MInput.Mouse.X + 10;
            int posY = MInput.Mouse.Y + 10;

            int backgroundWidth = tooltipsText.TextWidth + 16;
            int backgroundHeight = tooltipsText.TextHeight + 16;

            if(posX + backgroundWidth > Engine.Width)
            {
                posX = MInput.Mouse.X - backgroundWidth - 10;
            }

            if(posY + backgroundHeight > Engine.Height)
            {
                posY = MInput.Mouse.Y - backgroundHeight - 10;
            }

            tooltipsText.X = posX + 8;
            tooltipsText.Y = posY + 8;

            tooltipsBackground.X = posX;
            tooltipsBackground.Y = posY;

            tooltipsBackground.Width = backgroundWidth;
            tooltipsBackground.Height = backgroundHeight;
        }

    }
}
