using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CameraMovementScript : Component
    {

        private Vector2 mouseLastPosition;
        private const int MAX_SPEED = 1000;
        private const int DEFAULT_SPEED = 500;

        private int currentSpeed;

        private Vector2 lastCameraPosition = Vector2.Zero;
        private int lastCameraZoom = 1;

        private float minZoom = 1f;
        private float maxZoom = 4f;
        private float zoomDuration = 0.2f;

        private float zoomLevel = 1.0f;

        private bool ignoreUI;

        private Tweener tweener;

        private float smoothSpeed = 10f;
        private Vector2 ghostPlayerPosition;

        private Entity followEntity;

        public CameraMovementScript(bool ignoreUI) : base(true, false)
        {
            this.ignoreUI = ignoreUI;

            Engine.Instance.OnSceneLoadedCallback += OnSceneLoaded;

            currentSpeed = DEFAULT_SPEED;

            tweener = new Tweener();
        }

        private void SetZoomLevel(float value)
        {
            zoomLevel = MathHelper.Clamp(value, minZoom, maxZoom);
            tweener.TweenTo(target: RenderManager.MainCamera, expression: camera => RenderManager.MainCamera.Zoom, toValue: zoomLevel, 
                duration: zoomDuration, delay: 0)
                .Easing(EasingFunctions.SineOut);
        }

        private void OnSceneLoaded(Scene scene)
        {
            RenderManager.MainCamera.Zoom = lastCameraZoom;
            RenderManager.MainCamera.Position = lastCameraPosition;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            lastCameraPosition = RenderManager.MainCamera.Position;
            ghostPlayerPosition = RenderManager.MainCamera.Position;

            lastCameraZoom = (int)RenderManager.MainCamera.Zoom;
            zoomLevel = (int)RenderManager.MainCamera.Zoom;
        }

        public override void Update()
        {
            tweener.Update(Engine.DeltaTime);

            if (GameplayScene.OnGameMenu == false)
            {
                if(followEntity != null)
                {
                    RenderManager.MainCamera.Position = followEntity.Position;
                    lastCameraPosition = followEntity.Position;
                    ghostPlayerPosition = followEntity.Position;
                }

                KeyboardAndEdgeScrollingMovementUpdate();

                if (ignoreUI)
                {
                    DragMovementUpdate();
                    ZoomUpdate();
                }
                else if (GameplayScene.MouseOnUI == false)
                {
                    DragMovementUpdate();
                    ZoomUpdate();
                }
            }
        }

        private void DragMovementUpdate()
        {
            if (MInput.Mouse.PressedMiddleButton)
            {
                mouseLastPosition = new Vector2(MInput.Mouse.X / RenderManager.MainCamera.Zoom, MInput.Mouse.Y / RenderManager.MainCamera.Zoom);

                followEntity = null;
            }

            if (MInput.Mouse.CheckMiddleButton)
            {
                Vector2 mouseNewPosition = new Vector2(MInput.Mouse.X / RenderManager.MainCamera.Zoom, MInput.Mouse.Y / RenderManager.MainCamera.Zoom);

                RenderManager.MainCamera.Position = RenderManager.MainCamera.Position - (mouseNewPosition - mouseLastPosition);
                RenderManager.MainCamera.Position = Vector2.Clamp(RenderManager.MainCamera.Position, Vector2.Zero, new Vector2(GameplayScene.WorldSize * Engine.TILE_SIZE));

                mouseLastPosition = mouseNewPosition;

                lastCameraPosition = RenderManager.MainCamera.Position;
                ghostPlayerPosition = lastCameraPosition;
            }
        }

        private void KeyboardAndEdgeScrollingMovementUpdate()
        {
            Vector2 motion = Vector2.Zero;

            if (GameSettings.EdgeScrollingCamera)
            {
                if (GameplayScene.MouseOnUI == false)
                {
                    if (MInput.Mouse.X <= 10)
                        motion.X = -1;
                    else if (MInput.Mouse.X >= Engine.Width - 10)
                        motion.X = 1;

                    if (MInput.Mouse.Y <= 10)
                        motion.Y = -1;
                    else if (MInput.Mouse.Y >= Engine.Height - 10)
                        motion.Y = 1;
                }
            }

            if (MInput.Keyboard.Check(Keys.A, Keys.Left))
                motion.X = -1;
            else if (MInput.Keyboard.Check(Keys.D, Keys.Right))
                motion.X = 1;

            if (MInput.Keyboard.Check(Keys.W, Keys.Up))
                motion.Y = -1;
            else if (MInput.Keyboard.Check(Keys.S, Keys.Down))
                motion.Y = 1;

            if (MInput.Keyboard.Check(Keys.LeftShift, Keys.RightShift))
                currentSpeed = MAX_SPEED;
            else
                currentSpeed = DEFAULT_SPEED; 

            if (motion != Vector2.Zero)
            {
                motion.Normalize();

                ghostPlayerPosition += motion * currentSpeed * Engine.DeltaTime;

                ghostPlayerPosition = Vector2.Clamp(ghostPlayerPosition, Vector2.Zero, new Vector2(GameplayScene.WorldSize * Engine.TILE_SIZE));

                lastCameraPosition = RenderManager.MainCamera.Position;

                followEntity = null;
            }

            RenderManager.MainCamera.Position = Vector2.Lerp(RenderManager.MainCamera.Position, ghostPlayerPosition, smoothSpeed * Engine.DeltaTime);
        }

        private void ZoomUpdate()
        {
            int value = MInput.Mouse.WheelDelta;
            if (value < 0)
            {
                SetZoomLevel((int)RenderManager.MainCamera.Zoom - 1);

                lastCameraZoom = (int)RenderManager.MainCamera.Zoom;
            }
            else if (value > 0)
            {
                SetZoomLevel((int)Math.Ceiling(RenderManager.MainCamera.Zoom) + 1);

                lastCameraZoom = (int)RenderManager.MainCamera.Zoom;
            }
        }

        public void SetEntityToFollow(Entity entity)
        {
            followEntity = entity;
        }

    }
}
