using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SmokeManager
    {

        private List<Smoke> smokeList = new List<Smoke>();

        private MyTexture smokeSprite;

        public SmokeManager()
        {
            smokeSprite = ResourceManager.GetTexture("smoke");
        }

        public void AddSmoke(Vector2 position)
        {
            Smoke smoke = new Smoke();
            smoke.Position = position;
            smoke.Alpha = 1.0f;

            smokeList.Add(smoke);
        }

        public void Update()
        {
            for (int i = smokeList.Count - 1; i >= 0; i--)
            {
                Smoke smoke = smokeList[i];

                smoke.Position.X += 5f * Engine.GameDeltaTime / 2;
                smoke.Position.Y -= 25 * Engine.GameDeltaTime / 2;
                smoke.Alpha -= Engine.GameDeltaTime / 2;

                if (smoke.Alpha <= 0.0f)
                {
                    smokeList.Remove(smoke);
                }
            }
        }

        public void Render()
        {
            foreach (var smoke in smokeList)
            {
                smokeSprite.DrawCentered(smoke.Position, Color.White * smoke.Alpha);
            }
        }

        private class Smoke
        {
            public Vector2 Position;
            public float Alpha;
        }
    }
}