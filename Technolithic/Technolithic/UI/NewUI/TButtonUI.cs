using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class TButtonUI : MNode
    {
        public enum ButtonState
        {
            None,
            Default,
            Hovered,
            Pressed,
            Disabled
        }

        public event Action<TButtonUI> ButtonDown;
        public event Action<TButtonUI> ButtonUp;
        public event Action<TButtonUI> ButtonPressed;

        public MyTexture ButtonTexture { get; set; }
        public MyTexture Icon { get; set; }
        public MyTexture ExtraIcon { get; set; }

        public SoundEffectInstance SoundEffect { get; set; } = ResourceManager.ClickSoundEffect;

        private Dictionary<ButtonState, Color> stateColors = new()
        {
            { ButtonState.Default, Color.White },
            { ButtonState.Hovered, Color.White },
            { ButtonState.Pressed, Color.White },
            { ButtonState.Disabled, Color.White }
        };

        private ButtonState _currentState = ButtonState.Default;
        private Color _currentColor;

        public bool Disabled
        {
            get => _currentState == ButtonState.Disabled;
            set => _currentState = value ? ButtonState.Disabled : ButtonState.Default;
        }

        // TODO: придумать способ корректировать размер иконок
        public TButtonUI(Scene scene) : base(scene)
        {
            SetButtonColor(Color.White);
        }

        public override void Update(int mouseX, int mouseY)
        {
            base.Update(mouseX, mouseY);

            if (_currentState != ButtonState.Disabled)
            {
                if (Intersects(mouseX, mouseY))
                {
                    GameplayScene.MouseOnUI = true;

                    if (MInput.Mouse.PressedLeftButton)
                    {
                        ButtonDown?.Invoke(this);

                        _currentState = ButtonState.Pressed;
                    }

                    if (_currentState == ButtonState.Pressed)
                    {
                        ButtonPressed?.Invoke(this);

                        if (MInput.Mouse.ReleasedLeftButton)
                        {
                            ButtonUp?.Invoke(this);

                            SoundEffect?.Play();

                            _currentState = ButtonState.Hovered;
                        }
                    }
                    else
                    {
                        _currentState = ButtonState.Hovered;
                    }
                }
                else
                {
                    _currentState = ButtonState.Default;
                }
            }

            _currentColor = stateColors[_currentState];
        }

        public override void Render()
        {
            ButtonTexture?.Draw(new Rectangle(X, Y, Width, Height), Vector2.Zero, _currentColor, 0, SpriteEffects.None);
            
            Icon?.Draw(new Rectangle(X, Y, Width, Height), Vector2.Zero, _currentColor, 0, SpriteEffects.None);
            
            ExtraIcon?.Draw(new Rectangle(X, Y, Width, Height), Vector2.Zero, _currentColor, 0, SpriteEffects.None);

            base.Render();
        }

        public void SetButtonColor(Color color)
        {
            stateColors[ButtonState.Default] = color;
            stateColors[ButtonState.Hovered] = new Color(Color.Multiply(color, 0.7845f), 1f);
            stateColors[ButtonState.Pressed] = new Color(Color.Multiply(color, 0.5885f), 1f);
            stateColors[ButtonState.Disabled] = color * 0.5f;
        }

    }
}
