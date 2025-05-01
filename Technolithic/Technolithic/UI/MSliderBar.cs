using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MSliderBar : MNode
    {
        public Color BackColor = Color.Black;
        public Color FrontColor = Color.YellowGreen;

        private int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue == value)
                    return;

                maxValue = value;

                if (currentValue > maxValue)
                    currentValue = maxValue;

                UpdateFrontWidth();
            }
        }

        private int currentValue;
        public int CurrentValue
        {
            get { return currentValue; }
            set
            {
                if (currentValue == value)
                    return;

                currentValue = value;

                CurrentValueChanged?.Invoke(currentValue, this);

                UpdateFrontWidth();
            }
        }

        public Action<int, MSliderBar> CurrentValueChanged;

        public bool ShowDragger { get; set; } = true;
        public bool CheckInput { get; set; } = true;

        private Rectangle bounds = Rectangle.Empty;
        private Rectangle centerBounds = Rectangle.Empty;
        private int frontWidth;
        private bool dragging = false;
        private MyTexture leftTexture;
        private MyTexture centerTexture;
        private MyTexture rightTexture;
        private MyTexture draggerTexture;

        public MSliderBar(Scene scene, int width, int height) : base(scene)
        {
            Width = width;
            Height = height;
            bounds.Width = Width;
            bounds.Height = Height;
            centerBounds.Width = Width - 8;
            centerBounds.Height = Height;
            leftTexture = ResourceManager.LeftSliderBarTexture;
            centerTexture = ResourceManager.CenterSliderBarTexture;
            rightTexture = ResourceManager.RightSliderBarTexture;
            draggerTexture = ResourceManager.DraggerTexture;
        }

        public void SetCurrentValueSilent(int value)
        {
            if (currentValue == value)
                return;

            currentValue = value;

            UpdateFrontWidth();
        }

        private void UpdateFrontWidth()
        {
            float div = (float)currentValue / (float)MaxValue;
            frontWidth = (int)(centerBounds.Width * div);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (Active && CheckInput)
            {
                if (MInput.Mouse.PressedLeftButton)
                {
                    bounds.X = X;
                    bounds.Y = Y;

                    if (bounds.Contains(mouseX, mouseY))
                    {
                        dragging = true;
                    }
                }

                if (MInput.Mouse.ReleasedLeftButton)
                {
                    dragging = false;
                }

                if (dragging)
                {
                    bounds.X = X;
                    bounds.Y = Y;

                    centerBounds.X = X + 4;
                    centerBounds.Y = Y;

                    int diff = mouseX - centerBounds.X;
                    if (diff >= centerBounds.Width)
                    {
                        diff = centerBounds.Width;
                    }
                    else if (diff <= 0)
                    {
                        diff = 0;
                    }

                    float div = (float)diff / (float)centerBounds.Width;
                    CurrentValue = (int)(MaxValue * div);
                    frontWidth = (int)(centerBounds.Width * div);
                }
            }

            base.Update(mouseX, mouseY);
        }

        public override void Render()
        {
            if (Active)
            {
                leftTexture.Draw(new Rectangle(X, Y, 4, 16), Color.White);
                centerTexture.Draw(new Rectangle(X + 4, Y, Width - 8, 16), Color.White);
                rightTexture.Draw(new Rectangle(X + (Width - 8), Y, 4, 16), Color.White);

                RenderManager.Rect(X + 4, Y + 4, frontWidth, 8, FrontColor);

                if (ShowDragger)
                {
                    draggerTexture.Draw(new Rectangle(X + frontWidth, Y - 4, 6, 24), Color.White);
                }

                base.Render();
            }
        }

    }
}
