using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MImage : MNode
    {

        public Vector2 Origin;
        public float Rotation;
        public Color Color = Color.White;

        public MyTexture Texture;

        public SpriteEffects Effects = SpriteEffects.None;

        public MImage(Scene scene) : base(scene)
        { 
        }

        public MImage CenterOrigin()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;
            return this;
        }

        public override void Render()
        {
            if (Active)
            {
                Texture?.Draw(new Rectangle(X, Y, Width, Height), Origin, Color, Rotation, Effects);
            }

            base.Render();
        }

    }
}
