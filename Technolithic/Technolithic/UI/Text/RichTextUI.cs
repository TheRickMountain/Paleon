using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class RichTextUI : MNode
    {
        private RichTextLayout rtl;

        public string Text
        {
            get { return rtl.Text; }
            set
            {
                if (rtl.Text == value)
                    return;

                rtl.Text = value;

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

                rtl.Font = outlined ? RenderManager.StashOutlinedFont : RenderManager.StashDefaultFont;

                UpdateSizes();
            }
        }

        public int TextWidth { get; private set; } = 0;
        public int TextHeight { get; private set; } = 0;

        private SpriteFontBase font;

        public RichTextUI(Scene scene) : base(scene)
        {
            rtl = new RichTextLayout();
            rtl.Font = RenderManager.StashDefaultFont;
        }

        private void UpdateSizes()
        {
            TextWidth = rtl.Size.X;
            TextHeight = rtl.Size.Y;
        }

        public override void Render()
        {
            base.Render();

            if (Active)
            {
                rtl.Draw(RenderManager.SpriteBatch, new Vector2(X, Y), Color);
            }
        }
    }
}
