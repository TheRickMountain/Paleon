using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class BuildingsListUI : MyPanelUI
    {

        public BuildingsListUI(Scene scene) : base(scene, "", Color.White)
        {
            int columnsCount = 3;
            int rowsCount = 3;

            int buttonSize = 80;

            ListViewUI listView = new ListViewUI(scene, buttonSize, buttonSize, rowsCount, columnsCount, true, false);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;
            AddChildNode(listView);

            Width = listView.Width + 16; 
            Height = listView.Height + 16 + 32;

            AddComponent(new BuildingListUIScript());
        }

    }
}
