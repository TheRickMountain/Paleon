using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Technolithic
{
    public class MainCamera
    {
        public Vector2 Position { get; set; }

        public float Zoom { get; set; }

        public MainCamera()
        {
            Position = Vector2.Zero;
            Zoom = 1;
        }

        public Rectangle GetViewport()
        {
            return new Rectangle(
                (int)(Position.X - Engine.HalfWidth / Zoom),
                (int)(Position.Y - Engine.HalfHeight / Zoom),
                (int)(Engine.Width / Zoom), (int)(Engine.Height / Zoom));
        }

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-(int)Position.X, -(int)Position.Y, 0f))
                    * Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(new Vector3(Engine.HalfWidth, Engine.HalfHeight, 0));
            }
        }

    }
}
