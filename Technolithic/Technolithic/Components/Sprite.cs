using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public class Sprite : Component
    {
        public MyTexture Texture;

        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Scale = Vector2.One;
        public float Rotation;
        public Color Color = Color.White;
        public SpriteEffects Effects = SpriteEffects.None;

        private Rectangle dest;

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public virtual int Width
        {
            get { return dest.Width; }
            set { dest.Width = value; }
        }
        public virtual int Height
        {
            get { return dest.Height; }
            set { dest.Height = value; }
        }

        private bool flipX = false;
        public bool FlipX
        {
            get { return (Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }
            set
            {
                if (flipX != value)
                {
                    flipX = value;
                    Effects = value ? (Effects | SpriteEffects.FlipHorizontally) : (Effects & ~SpriteEffects.FlipHorizontally);
                }
            }
        }

        private bool flipY = false;
        public bool FlipY
        {
            get { return (Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }
            set
            {
                if (flipY != value)
                {
                    flipY = value;
                    Effects = value ? (Effects | SpriteEffects.FlipVertically) : (Effects & ~SpriteEffects.FlipVertically);
                }
            }
        }

        public Vector2 RenderPosition
        {
            get { return (Entity == null ? Vector2.Zero : Entity.Position) + Position; }
            set { Position = value - (Entity == null ? Vector2.Zero : Entity.Position); }
        }

        public Sprite(MyTexture texture)
            : base(false, true)
        {
            Texture = texture;
            dest.Width = texture.Width;
            dest.Height = texture.Height;
        }

        public Sprite(MyTexture texture, int width, int height)
            : base(false, true)
        {
            Texture = texture;

            dest.Width = width;
            dest.Height = height;
        }
        
        public Sprite(MyTexture texture, int width, int height, bool active)
            : base(active, true)
        {
            Texture = texture;
            dest.Width = width;
            dest.Height = height;
        }

        public Sprite SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
            return this;
        }

        public Sprite CenterOrigin()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;
            return this;
        }

        public override void Render()
        {
            if (Texture != null)
            {
                dest.X = (int)RenderPosition.X;
                dest.Y = (int)RenderPosition.Y;
                Texture.Draw(dest, Origin, Color, Rotation, Effects);
            }
        }

        public void TestRender()
        {
            Texture?.Draw(RenderPosition, Origin, Color, Scale, Rotation, Effects);
        }

        public virtual void Render(int x, int y, float scale)
        {
            if(Texture != null)
            {
                Texture.Draw(new Rectangle(x, y, (int)(dest.Width * scale), (int)(dest.Height * scale)), Origin, Color, Rotation, Effects);
            }
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }

        public void SetTextureSize()
        {
            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;
            }
        }
    }

}
