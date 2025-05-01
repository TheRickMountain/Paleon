using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ToggleScript : MScript
    {

        public MImageCmp BackgroundImage { get; set; }

        public Color NormalColor { get; set; } = new Color(255, 255, 255);
        public Color HighlightedColor { get; set; } = new Color(200, 200, 200);
        public Color PressedColor { get; set; } = new Color(150, 150, 150);

        private bool radio;

        private Action<bool, MToggleUI> onValueChanged;

        private bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                if (isOn == value)
                    return;

                isOn = value;

                if (isOn)
                    BackgroundImage.Texture = toggledTexture;
                else
                    BackgroundImage.Texture = untoggledTexture;

                onValueChanged?.Invoke(isOn, (MToggleUI)ParentNode);
            }
        }

        private MyTexture toggledTexture;
        private MyTexture untoggledTexture;

        public ToggleScript(bool isOn, bool radio) : base(true)
        {
            this.isOn = isOn;
            this.radio = radio;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            if (radio)
            {
                toggledTexture = TextureBank.UITexture.GetSubtexture(160, 64, 16, 16);
            }
            else
            {
                toggledTexture = TextureBank.UITexture.GetSubtexture(192, 48, 16, 16);
            }

            untoggledTexture = TextureBank.UITexture.GetSubtexture(176, 48, 16, 16);

            BackgroundImage.Texture = untoggledTexture;
            BackgroundImage.Color = NormalColor;

            if (isOn)
                BackgroundImage.Texture = toggledTexture;
            else
                BackgroundImage.Texture = untoggledTexture;
        }

        public override void Update(int mouseX, int mouseY)
        {
            BackgroundImage.Color = NormalColor;

            if(BackgroundImage.Bound.Contains(mouseX, mouseY))
            {
                BackgroundImage.Color = HighlightedColor;

                if (MInput.Mouse.ReleasedLeftButton)
                    IsOn = !IsOn;

                if (MInput.Mouse.CheckLeftButton)
                    BackgroundImage.Color = PressedColor;
            }
        }

        public void SilentCheck(bool value)
        {
            this.isOn = value;

            if (isOn)
                BackgroundImage.Texture = toggledTexture;
            else
                BackgroundImage.Texture = untoggledTexture;
        }

        public void AddOnValueChangedCallback(Action<bool, MToggleUI> callback)
        {
            onValueChanged += callback;
        }

        public void RemoveOnValueChangedCallback(Action<bool, MToggleUI> callback)
        {
            onValueChanged -= callback;
        }

    }
}
