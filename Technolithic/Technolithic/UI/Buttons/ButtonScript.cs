using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ButtonScript : MScript
    {

        public MImageCmp BackgroundImage { get; set; }

        public Color NormalColor { get; private set; } = new Color(255, 255, 255);
        public Color HighlightedColor { get; private set; } = new Color(200, 200, 200);
        public Color PressedColor { get; private set; } = new Color(150, 150, 150);

        public Color SelectedNormalColor { get; private set; } = Color.Orange;
        public Color SelectedHighlightedColor { get; private set; } = Utils.Blend(new Color(200, 200, 200), Color.Orange);
        public Color SelectedPressedColor { get; private set; } = Utils.Blend(new Color(150, 150, 150), Color.Orange);

        public Color DisabledColor { get; private set; } = new Color(128, 128, 128);

        public SoundEffectInstance SoundEffect { get; set; } = ResourceManager.ClickSoundEffect;

        private Color _normalColor = Color.White;
        private Color _highlightedColor = Color.White;
        private Color _pressedColor = Color.White;

        public bool IsSelectable { get; set; } = false;

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            { 
                isSelected = value;

                if (isSelected)
                {
                    _normalColor = SelectedNormalColor;
                    _highlightedColor = SelectedHighlightedColor;
                    _pressedColor = SelectedPressedColor;

                    BackgroundImage.Color = _normalColor;
                }
                else
                {
                    _normalColor = NormalColor;
                    _highlightedColor = HighlightedColor;
                    _pressedColor = PressedColor;

                    BackgroundImage.Color = _normalColor;
                }
            }
        }

        public bool IsHovered { get; private set; } = false;

        private bool isDisabled;
        public bool IsDisabled 
        { 
            get { return isDisabled; } 
            set
            {
                if (isDisabled == value)
                    return;

                isDisabled = value;

                if(isDisabled)
                {
                    _normalColor = DisabledColor;
                    _highlightedColor = DisabledColor;
                    _pressedColor = DisabledColor;

                    BackgroundImage.Color = _normalColor;
                }
                else
                {
                    IsSelected = IsSelected;
                }

                foreach(var child in ParentNode.GetChidlrenEnumerable())
                {
                    MImageCmp childImage = child.GetComponent<MImageCmp>();

                    if (childImage != null)
                    {
                        if (isDisabled)
                            childImage.Color = DisabledColor;
                        else
                            childImage.Color = Color.White;
                    }

                    if (child is MyText)
                    {
                        MyText childText = child as MyText;

                        if (isDisabled)
                            childText.Color = DisabledColor;
                        else
                            childText.Color = Color.White;
                    }
                }
            }
        }

        private Action<bool, ButtonScript> onClicked;
        public Action<ButtonScript> ButtonChecked;
        private Action<ButtonScript> onHovered;

        public event Action<ButtonScript> Pressed;

        public bool AllowLeftClick = true;
        public bool AllowRightClick = false;

        public bool LeftButtonDetected { get; private set; }
        public bool RightButtonDetected { get; private set; }

        public ButtonScript() : base(true)
        {
            
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            if(isDisabled)
            {
                _normalColor = DisabledColor;
                _highlightedColor = DisabledColor;
                _pressedColor = DisabledColor;
            }
            else if(isSelected)
            {
                _normalColor = SelectedNormalColor;
                _highlightedColor = SelectedHighlightedColor;
                _pressedColor = SelectedPressedColor;
            }
            else
            {
                _normalColor = NormalColor;
                _highlightedColor = HighlightedColor;
                _pressedColor = PressedColor;
            }

            BackgroundImage.Color = _normalColor;
        }

        public override void Update(int mouseX, int mouseY)
        {
            IsHovered = false;

            if (BackgroundImage.Bound.Contains(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;

                onHovered?.Invoke(this);
                IsHovered = true;

                if (IsDisabled == false)
                {
                    BackgroundImage.Color = _highlightedColor;

                    if (AllowLeftClick)
                    {
                        if (MInput.Mouse.PressedLeftButton)
                        {
                            LeftButtonDetected = true;
                        }

                        if (LeftButtonDetected && MInput.Mouse.CheckLeftButton)
                        {
                            ButtonChecked?.Invoke(this);
                        }

                        if (LeftButtonDetected && MInput.Mouse.ReleasedLeftButton)
                        {
                            if (IsSelectable)
                                IsSelected = !IsSelected;

                            SoundEffect.Play();

                            onClicked?.Invoke(isSelected, this);
                            Pressed?.Invoke(this);

                            LeftButtonDetected = false;
                        }
                    }

                    if (AllowRightClick)
                    {
                        if (MInput.Mouse.PressedRightButton)
                        {
                            RightButtonDetected = true;
                        }

                        if (RightButtonDetected && MInput.Mouse.CheckRightButton)
                        {
                            ButtonChecked?.Invoke(this);
                        }

                        if (RightButtonDetected && MInput.Mouse.ReleasedRightButton)
                        {
                            if (IsSelectable)
                                IsSelected = !IsSelected;

                            SoundEffect.Play();

                            onClicked?.Invoke(isSelected, this);

                            RightButtonDetected = false;
                        }
                    }

                    if (MInput.Mouse.CheckLeftButton)
                    {
                        BackgroundImage.Color = _pressedColor;
                    }
                }
            }
            else
            {
                LeftButtonDetected = false;
            }

            if (IsHovered == false)
            {
                BackgroundImage.Color = _normalColor;
            }
        }

        public void SetDefaultColor(Color normalColor, Color highlightedColor, Color pressedColor, bool blend = true)
        {
            NormalColor = normalColor;

            if (blend)
            {
                HighlightedColor = Utils.Blend(new Color(200, 200, 200), highlightedColor);
                PressedColor = Utils.Blend(new Color(150, 150, 150), pressedColor);
            }
            else
            {
                HighlightedColor = highlightedColor;
                PressedColor = pressedColor;
            }

            if (!isSelected)
            {
                _normalColor = NormalColor;
                _highlightedColor = HighlightedColor;
                _pressedColor = PressedColor;
            }
        }

        public void AddOnClickedCallback(Action<bool, ButtonScript> callback)
        {
            onClicked += callback;
        }

        public void RemoveOnClickedCallback(Action<bool, ButtonScript> callback)
        {
            onClicked -= callback;
        }

        public void AddOnHoveredCallback(Action<ButtonScript> callback)
        {
            onHovered += callback;
        }

        public void RemoveOnHoveredCallback(Action<ButtonScript> callback)
        {
            onHovered -= callback;
        }
    }
}
