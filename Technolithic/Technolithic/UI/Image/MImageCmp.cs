using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum ImageType
    {
        Simple,
        Sliced
    }

    public class MImageCmp : MComponent
    {
        public string Name;
        public MyTexture Texture;
        public Vector2 Origin;
        public float Rotation;
        public Color Color = Color.White;

        public ImageType ImageType { get; set; } = ImageType.Simple;
        private Rectangle[] nineSliced = new Rectangle[9];

        private int top;
        private int left;
        private int right;
        private int bottom;

        public SpriteEffects Effects = SpriteEffects.None;

        public int BackgroundOverlap { get; set; } = 0;

        public MImageCmp() : base(false, true)
        {
        }

        public MImageCmp CenterOrigin()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;
            return this;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Update(int mouseX, int mouseY)
        {
        }

        public override void Render()
        {
            if (Visible)
            {
                switch(ImageType)
                {
                    case ImageType.Simple:
                        {
                            Texture?.Draw(new Rectangle(ParentNode.X, ParentNode.Y, ParentNode.Width, ParentNode.Height),
                                    Origin, Color, Rotation, Effects);
                        }
                        break;
                    case ImageType.Sliced:
                        {
                            int centerWidth = ParentNode.Width - left - right;
                            if (centerWidth < 0)
                                centerWidth = 0;

                            int centerHeight = ParentNode.Height - top - bottom;
                            if (centerHeight < 0)
                                centerHeight = 0;

                            // CENTER
                            Texture?.Draw(new Rectangle(
                                (ParentNode.X + left) - BackgroundOverlap,
                                (ParentNode.Y + top) - BackgroundOverlap,
                                centerWidth + BackgroundOverlap + BackgroundOverlap,
                                centerHeight + BackgroundOverlap + BackgroundOverlap),
                               nineSliced[4], Origin, Color, Rotation, Effects);

                            // TOP - LEFT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X, 
                                ParentNode.Y, 
                                left, 
                                top),
                                nineSliced[0], Origin, Color, Rotation, Effects);

                            // TOP
                            Texture?.Draw(new Rectangle(
                                ParentNode.X + left, 
                                ParentNode.Y,
                                centerWidth, 
                                top),
                                nineSliced[1], Origin, Color, Rotation, Effects);

                            // TOP - RIGHT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X + left + centerWidth, 
                                ParentNode.Y, 
                                right, 
                                top),
                                nineSliced[2], Origin, Color, Rotation, Effects);

                            // LEFT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X, 
                                ParentNode.Y + top, 
                                left,
                                centerHeight),
                               nineSliced[3], Origin, Color, Rotation, Effects);

                            // RIGHT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X + left + centerWidth, 
                                ParentNode.Y + top, 
                                right,
                                centerHeight),
                               nineSliced[5], Origin, Color, Rotation, Effects);

                            // BOTTOM - LEFT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X, 
                                ParentNode.Y + top + centerHeight, 
                                left, 
                                bottom),
                                nineSliced[6], Origin, Color, Rotation, Effects);

                            // BOTTOM
                            Texture?.Draw(new Rectangle(
                                ParentNode.X + left, 
                                ParentNode.Y + top + centerHeight, 
                                centerWidth, 
                                bottom),
                                nineSliced[7], Origin, Color, Rotation, Effects);

                            // BOTTOM - RIGHT
                            Texture?.Draw(new Rectangle(
                                ParentNode.X + left + centerWidth, 
                                ParentNode.Y + top + centerHeight, 
                                right, 
                                bottom),
                                nineSliced[8], Origin, Color, Rotation, Effects);
                        }
                        break;
                } 
            }
        }

        public void SetBorder(int top, int left, int right, int bottom)
        {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;

            int centerWidth = Texture.Width - left - right;
            int centerHeight = Texture.Height - top - bottom;

            nineSliced[0] = Texture.GetRelativeRect(new Rectangle(0,                  0,                  left,        top));
            nineSliced[1] = Texture.GetRelativeRect(new Rectangle(left,               0,                  centerWidth, top));
            nineSliced[2] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, 0,                  right,       top));

            nineSliced[3] = Texture.GetRelativeRect(new Rectangle(0,                  top,                left,        centerHeight));
            nineSliced[4] = Texture.GetRelativeRect(new Rectangle(left,               top,                centerWidth, centerHeight));
            nineSliced[5] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, top,                right,       centerHeight));

            nineSliced[6] = Texture.GetRelativeRect(new Rectangle(0,                  top + centerHeight, left,        bottom));
            nineSliced[7] = Texture.GetRelativeRect(new Rectangle(left,               top + centerHeight, centerWidth, bottom));
            nineSliced[8] = Texture.GetRelativeRect(new Rectangle(left + centerWidth, top + centerHeight, right,       bottom));
        }

        public Rectangle Bound
        {
            get { return new Rectangle(ParentNode.X, ParentNode.Y, ParentNode.Width, ParentNode.Height); }
        }
    }
}
