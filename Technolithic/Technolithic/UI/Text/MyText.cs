using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class MyText : MNode
    {

        private string text = "";
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;

                text = value;
                UpdateSizes();
            }
        }

        public Color Color { get; set; } = Color.White;

        private bool outlined = false;
        public bool Outlined
        {
            get => outlined;
            set
            {
                outlined = value;

                font = outlined ? RenderManager.StashOutlinedFont : RenderManager.StashDefaultFont;

                UpdateSizes();
            }
        }

        public int TextWidth { get; private set; } = 0;
        public int TextHeight { get; private set; } = 0;

        private SpriteFontBase font;

        public MyText(Scene scene) : base(scene)
        {
            font = RenderManager.StashDefaultFont;
        }

        private void UpdateSizes()
        {
            Vector2 textSize = font.MeasureString(Text);
            TextWidth = (int)textSize.X;
            TextHeight = (int)textSize.Y;
        }

        public override void Render()
        {
            base.Render();

            if (Active)
            {
                font.DrawText(RenderManager.SpriteBatch, text, new Vector2(X, Y), Color);
            }
        }
    }
}
