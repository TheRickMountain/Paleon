using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class TileLayerUI : MyPanelUI
    {
        private const int Padding = 8;
        private const int TitleHeight = 32;

        public RichTextUI Info { get; }
        public ListViewUI Buttons { get; }

        public TileLayerUI(Scene scene) : base(scene, string.Empty, Color.White)
        {
            Width = 280;

            Info = new RichTextUI(scene);
            Info.X = Padding;
            Info.Y = Padding + TitleHeight;
            AddChildNode(Info);

            Buttons = new ListViewUI(scene, 48, 48, 1, 5, false, false);
            Buttons.X = Padding;
            AddChildNode(Buttons);

            FitHeight();

            AddComponent(new TileLayerUIScript());
        }

        public void FitHeight()
        {
            int infoHeight = string.IsNullOrEmpty(Info.Text) ? 0 : Info.TextHeight + Padding;

            Height = Padding + TitleHeight + infoHeight + Padding + Buttons.Height + Padding;
            Buttons.Y = Height - Buttons.Height - Padding;
        }
    }
}
