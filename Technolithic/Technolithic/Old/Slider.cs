using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Slider
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;

                back.X = (int)position.X - 1;
                back.Y = (int)position.Y - 1;

                front.X = (int)position.X;
                front.Y = (int)position.Y;
            }
        }

        public bool Active {get; set;} = true;

        private Sprite back;
        private Sprite front;

        public Slider(int width, int height, Color backColor, Color frontColor)
        {
            back = new Sprite(RenderManager.Pixel, width + 2, height + 2);
            front = new Sprite(RenderManager.Pixel, 0, height);

            back.Color = backColor;
            front.Color = frontColor;

            Position = new Vector2(0, 0);
        }

        public void Render()
        {
            if (Active)
            {
                back.Render();
                front.Render();
            }
        }

        public void SetValue(float min, float max, float current, Color frontColor)
        {
            front.Color = frontColor;

            float total = max - min;
            float curr = max - current; 

            float percent = MathHelper.Clamp(1.0f - (curr / total), 0, 1);

            front.Width = (int)(percent * back.Width - 1);
        }

    }
}
