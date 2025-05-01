using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class AssignPetUI : MyPanelUI
    {

        public AssignPetUI(Scene scene) : base(scene, Localization.GetLocalizedText("pets"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 300, 48, 6);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            AddChildNode(listView);

            Width = listView.Width + 16;
            Height = listView.Height + 96;

            AddComponent(new AssignPetUIScript());
        }

    }
}
