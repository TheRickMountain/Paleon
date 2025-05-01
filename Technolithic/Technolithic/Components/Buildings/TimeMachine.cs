using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum TimeMachineStates
    {
        Shake,
        Explosion,
        Disappearance,
        Disappeared,
        None
    }

    public class TimeMachine : BuildingCmp
    {
        // enum states
        // дрожь
        // молнии
        // машина времени исчезает

        public bool ReturnHome { get; set; } = false;

        private TimeMachineStates currentState = TimeMachineStates.None;

        private float spriteAlpha = 1.0f;
        private float shakeProgress = 0.0f;
        private float shakeTime = 5.0f;

        private Vector2 spriteStartPosition;

        private AnimatedSprite explosionSprite;

        public TimeMachine(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {

        }

        public override void Begin()
        {
            base.Begin();

            spriteStartPosition = Sprite.Position;

            ThrowBuildingRecipeItemsAfterDestructing = false;

            explosionSprite = new AnimatedSprite(48, 48);
            explosionSprite.Add("explosion", new Animation(ResourceManager.GetTexture("explosion"), 4, 0, 48, 48, 0, 48));
            explosionSprite.Play("explosion");

            explosionSprite.Entity = Entity;
        }

        public override void UpdateCompleted()
        {
            if(ReturnHome)
            {
                explosionSprite.Update();
            }

            base.UpdateCompleted();

            switch(currentState)
            {
                case TimeMachineStates.None:
                    {
                        if(ReturnHome)
                        {
                            currentState = TimeMachineStates.Shake;
                        }
                    }
                    break;
                case TimeMachineStates.Shake:
                    {
                        Sprite.Position.X = spriteStartPosition.X + (float)Math.Sin(shakeProgress * 40.0f) * 2.0f;

                        shakeProgress += Engine.GameDeltaTime;

                        if(shakeProgress >= shakeTime)
                        {
                            currentState = TimeMachineStates.Explosion;
                            shakeProgress = 0.0f;
                        }
                    }
                    break;
                case TimeMachineStates.Explosion:
                    {
                        Sprite.Position.X = spriteStartPosition.X + (float)Math.Sin(shakeProgress * 40.0f) * 2.0f;

                        shakeProgress += Engine.GameDeltaTime;

                        if (shakeProgress >= shakeTime)
                        {
                            currentState = TimeMachineStates.Disappearance;
                            shakeProgress = 0.0f;
                        }
                    }
                    break;
                case TimeMachineStates.Disappearance:
                    {
                        Sprite.Position.X = spriteStartPosition.X + (float)Math.Sin(shakeProgress * 40.0f) * 2.0f;

                        shakeProgress += Engine.GameDeltaTime;

                        spriteAlpha -= Engine.GameDeltaTime;
                        Sprite.Color = new Color(spriteAlpha, spriteAlpha, spriteAlpha, spriteAlpha);

                        if(spriteAlpha <= 0)
                        {
                            currentState = TimeMachineStates.Disappeared;
                        }
                    }
                    break;
                case TimeMachineStates.Disappeared:
                    {
                        DestructBuilding();

                        GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                            .AddNotification(Localization.GetLocalizedText("game_finished") + "!");

                        GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.BACK_TO_THE_FUTURE);
                    }
                    break;
            }
        }

        public override void Render()
        {
            base.Render();

            switch(currentState)
            {
                case TimeMachineStates.Explosion:
                case TimeMachineStates.Disappearance:
                    {
                        explosionSprite.Render();
                    }
                    break;
            }
        }

    }
}
