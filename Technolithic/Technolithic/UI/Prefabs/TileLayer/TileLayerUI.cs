using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class TileLayerUI : MyPanelUI
    {
        private const int Padding = 8;
        private const int TitleHeight = 32;

        public ListViewUI Buttons { get; }

        public TileLayerUI(Scene scene) : base(scene, string.Empty, Color.White)
        {
            Width = 280;

            Buttons = new ListViewUI(scene, 48, 48, 1, 5, false, false);
            Buttons.X = Padding;
            AddChildNode(Buttons);

            FitHeight();

            AddComponent(new TileLayerUIScript());
        }

        public void FitHeight()
        {
            Height = Padding + TitleHeight + Padding + Buttons.Height + Padding;
            Buttons.Y = Height - Buttons.Height - Padding;
        }
    }
}
