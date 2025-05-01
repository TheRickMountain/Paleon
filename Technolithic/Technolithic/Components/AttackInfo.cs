using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AttackInfo
    {

        private CreatureCmp hunter;
        private CreatureCmp victim;
        private Vector2 hunterPosition;
        private Vector2 victimPosition;
        private Vector2 difference;
        private Vector2 arrowPosition;
        private Sprite ammunitionImage;
        private Tool tool;
        private int hitPercentage = 80;
        private float arrowAngle;

        private float flyStep;
        private float flyProgress = 0;

        public bool IsCompleted { get; private set; }

        private ParticleEngine particleEngine;

        private bool isArrowReachedTarget;

        public AttackInfo(CreatureCmp hunter, CreatureCmp victim, Tool tool)
        {
            this.hunter = hunter;
            this.victim = victim;
            this.tool = tool;

            hunterPosition = hunter.Entity.Get<SelectableCmp>().BoundingBox.Center.ToVector2();
            victimPosition = victim.Entity.Get<SelectableCmp>().BoundingBox.Center.ToVector2();

            ammunitionImage = new Sprite(tool.AmmoTexture);

            difference = victimPosition - hunterPosition;
            difference.Normalize();

            arrowAngle = (float)Math.Atan2(difference.Y, difference.X);

            int distance = Utils.GetDistance(hunterPosition, victimPosition);

            flyStep = 1.0f / distance;

            List<MyTexture> textures = new List<MyTexture>();
            textures.Add(RenderManager.Pixel);

            particleEngine = new ParticleEngine(textures, hunterPosition);
        }

        public void Update()
        {
            if (!IsCompleted)
            {
                if (!isArrowReachedTarget)
                {
                    arrowPosition = Vector2.Lerp(hunterPosition, victimPosition, flyProgress);
                    flyProgress += flyStep * Engine.GameDeltaTime * 500 * tool.ProjectileSpeed;

                    ammunitionImage.Rotation = arrowAngle;
                    ammunitionImage.Position = arrowPosition;

                    if (flyProgress > 1.0f)
                    {
                        isArrowReachedTarget = true;
                        particleEngine.IsStopped = true;

                        if (MyRandom.Range(0, 100) < hitPercentage)
                        {
                            victim.TakeDamage(hunter, tool.RangeDamage);
                        }
                    }
                }

                particleEngine.EmitterLocation = arrowPosition;
                particleEngine.Update();

                if (particleEngine.Count == 0)
                    IsCompleted = true;
            }
        }

        public void Render()
        {
            if (!IsCompleted)
            {
                if (!isArrowReachedTarget)
                    ammunitionImage.Render();

                particleEngine.Draw();
            }
        }

    }
}
