using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Particle
    {
        public MyTexture Texture { get; set; }     
        public Vector2 Position { get; set; }              
        public Vector2 Velocity { get; set; }       
        public float Angle { get; set; }            
        public float AngularVelocity { get; set; }   
        public Color Color { get; set; }          
        public float Size { get; set; }               
        public float TTL { get; set; }

        public Particle(MyTexture texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, float ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            Size -= Engine.GameDeltaTime * 4;
            Position += Velocity * Engine.GameDeltaTime * 60;
            Angle += AngularVelocity * Engine.GameDeltaTime * 60;
        }

        public void Draw()
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            Texture.Draw(Position, origin, Color, Size, Angle);
        }
    }
}
