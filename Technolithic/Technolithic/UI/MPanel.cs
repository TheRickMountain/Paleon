using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MPanel : MNode
    {
        private MyTexture texture;
        public MyTexture Texture 
        { 
            get { return texture; } 
            set
            {
                texture = value;

                UpdateNineSlices();
            }
        }

        public Color Color { get; set; } = Color.White;

        public bool RenderCenter { get; set; } = true;

        private Rectangle[] nineSliced = new Rectangle[9];

        private int top = 0;
        private int left = 0;
        private int right = 0;
        private int bottom = 0;

        public MPanel(Scene scene) : base(scene)
        {

        }

        public override void Render()
        {
            if (Active && Texture != null)
            {
                int centerWidth = Width - left - right;
                if (centerWidth < 0)
                    centerWidth = 0;

                int centerHeight = Height - top - bottom;
                if (centerHeight < 0)
                    centerHeight = 0;

                if (RenderCenter)
                {
                    // CENTER
                    Texture.Draw(new Rectangle(X + left, Y + top, centerWidth, centerHeight),
                       nineSliced[4], Vector2.Zero, Color, 0, SpriteEffects.None);
                }

                // TOP - LEFT
                Texture.Draw(new Rectangle(X,Y,left,top),
                    nineSliced[0], Vector2.Zero, Color, 0, SpriteEffects.None);

                // TOP
                Texture.Draw(new Rectangle(X + left, Y, centerWidth, top),
                    nineSliced[1], Vector2.Zero, Color, 0, SpriteEffects.None);

                // TOP - RIGHT
                Texture.Draw(new Rectangle(X + left + centerWidth, Y, right, top),
                    nineSliced[2], Vector2.Zero, Color, 0, SpriteEffects.None);

                // LEFT
                Texture.Draw(new Rectangle(X, Y + top, left, centerHeight),
                   nineSliced[3], Vector2.Zero, Color, 0, SpriteEffects.None);

                // RIGHT
                Texture.Draw(new Rectangle(X + left + centerWidth, Y + top, right, centerHeight),
                   nineSliced[5], Vector2.Zero, Color, 0, SpriteEffects.None);

                // BOTTOM - LEFT
                Texture.Draw(new Rectangle(X,Y + top + centerHeight, left, bottom),
                    nineSliced[6], Vector2.Zero, Color, 0, SpriteEffects.None);

                // BOTTOM
                Texture.Draw(new Rectangle(X + left, Y + top + centerHeight, centerWidth, bottom),
                    nineSliced[7], Vector2.Zero, Color, 0, SpriteEffects.None);

                // BOTTOM - RIGHT
                Texture.Draw(new Rectangle(X + left + centerWidth, Y + top + centerHeight, right, bottom),
                    nineSliced[8], Vector2.Zero, Color, 0, SpriteEffects.None);
            }

            base.Render();
        }

        public void SetBorders(int top, int left, int right, int bottom)
        {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;

            if (Texture != null)
            {
                UpdateNineSlices();
            }
        }

        private void UpdateNineSlices()
        {
            int centerWidth = Texture.Width - left - right;
            int centerHeight = Texture.Height - top - bottom;

            nineSliced[0] = Texture.GetRelativeRect(new Rectangle(0, 0, left, top));
            nineSliced[1] = Texture.GetRelativeRect(new Rectangle(left, 0, centerWidth, top));
            nineSliced[2] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, 0, right, top));

            nineSliced[3] = Texture.GetRelativeRect(new Rectangle(0, top, left, centerHeight));
            nineSliced[4] = Texture.GetRelativeRect(new Rectangle(left, top, centerWidth, centerHeight));
            nineSliced[5] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, top, right, centerHeight));

            nineSliced[6] = Texture.GetRelativeRect(new Rectangle(0, top + centerHeight, left, bottom));
            nineSliced[7] = Texture.GetRelativeRect(new Rectangle(left, top + centerHeight, centerWidth, bottom));
            nineSliced[8] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, top + centerHeight, right, bottom));
        }

    }
}
