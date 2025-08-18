using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class TShortTextButtonUI : TBigButtonUI
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
            }
        }

        public TShortTextButtonUI(Scene scene) : base(scene)
        {
            ButtonTexture = AssetManager.GetTexture("Sprites", "short_text_button_ui");
            Width = ButtonTexture.Width * 2;
            Height = ButtonTexture.Height * 2;
        }

        public override void Render()
        {
            base.Render();

            RenderManager.StashDefaultFont.DrawText(RenderManager.SpriteBatch, _text, new Vector2(X + 8, Y + 8), Color.White);
        }
    }
}
