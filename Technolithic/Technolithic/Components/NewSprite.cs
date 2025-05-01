using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class NewSprite : Component
    {
        private MyTexture texture;
        public MyTexture Texture 
        {
            get => texture; 
            set
            {
                texture = value;

                UpdateData();
            }
        }

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        private bool isCentered = false;
        public bool IsCentered
        {
            get => isCentered;
            set
            {
                isCentered = value;

                UpdateData();
            }
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

        public NewSprite() : base(false, true)
        {

        }

        private void UpdateData()
        {
            if(isCentered && texture != null)
            {
                Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
            else
            {
                Origin = Vector2.Zero;
            }
        }

        public Vector2 RenderPosition
        {
            get { return (Entity == null ? Vector2.Zero : Entity.Position) + Position; }
            set { Position = value - (Entity == null ? Vector2.Zero : Entity.Position); }
        }


        public override void Render()
        {
            Texture?.Draw(RenderPosition, Origin, Color, Scale, Rotation, Effects);
        }

    }
}
