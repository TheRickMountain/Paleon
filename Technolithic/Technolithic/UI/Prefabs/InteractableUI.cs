using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class InteractableUI : MyPanelUI
    {

        public InteractableUI(Scene scene) : base(scene, "Test", Color.White)
        {
            Width = 250;
            Height = 200;

            ListViewUI buttonsListView = new ListViewUI(Scene, 48, 48, 1, 5, false, false);
            buttonsListView.Name = "ButtonsListView";
            buttonsListView.X = 8;
            buttonsListView.Y = Height - 48 - 8;
            AddChildNode(buttonsListView);

            AddComponent(new InteractableUIScript());
        }

    }
}
