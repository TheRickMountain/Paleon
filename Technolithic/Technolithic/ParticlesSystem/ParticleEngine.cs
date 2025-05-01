using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ParticleEngine
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<MyTexture> textures;

        public int Count => particles.Count;

        public bool IsStopped { get; set; }

        public ParticleEngine(List<MyTexture> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        private Particle GenerateNewParticle()
        {
            MyTexture texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            int ttl = 10;

            return new Particle(texture, position, Vector2.Zero, 0, angularVelocity, Color.White, 3, ttl);
        }

        public void Update()
        {
            if (!IsStopped)
            {
                int total = 5;

                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle());
                }
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Size <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Draw()
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw();
            }
        }
    }
}
