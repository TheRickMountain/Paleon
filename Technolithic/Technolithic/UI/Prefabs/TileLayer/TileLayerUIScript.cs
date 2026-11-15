using Microsoft.Xna.Framework.Input;

namespace Technolithic
{
    // Panel of a wall, floor, canal or farmland the player clicked on.
    public class TileLayerUIScript : MScript
    {
        private Tile tile;
        private ITileLayer layer;
        private BuildingTemplate template;

        private BigButton destructButton;
        private BigButton pipetteButton;

        public TileLayerUIScript() : base(true)
        {
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            destructButton = new BigButton(ParentNode.Scene, ResourceManager.DestructIcon, true);
            destructButton.GetComponent<ButtonScript>().AddOnClickedCallback(ToggleDestruction);
            destructButton.Tooltips = Localization.GetLocalizedText("destruct");

            pipetteButton = new BigButton(ParentNode.Scene, ResourceManager.PipetteIcon, false);
            pipetteButton.GetComponent<ButtonScript>().AddOnClickedCallback(Pick);
            pipetteButton.Tooltips = Localization.GetLocalizedText("clone") + " [Q]";
        }

        public void SetLayer(Tile tile, ITileLayer layer)
        {
            this.tile = tile;
            this.layer = layer;
            template = layer.GetTemplate(tile);

            TileLayerUI panel = (TileLayerUI)ParentNode;
            panel.LabelUI.Text = template.Name;
            panel.FitHeight();

            ListViewUIScript buttons = panel.Buttons.GetComponent<ListViewUIScript>();
            buttons.Clear();

            if (TileLayerDestruction.CanMark(tile, layer))
                buttons.AddItem(destructButton);

            if (BuildingPipette.CanPick(template, GameplayScene.Instance.ProgressTree))
                buttons.AddItem(pipetteButton);

            SyncDestructButton();
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (layer.IsPresent(tile) == false)
            {
                GameplayScene.UIRootNodeScript.CloseEntityPanel();
                return;
            }

            SyncDestructButton();

            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;

                // Over the world, Q picks what is under the cursor instead.
                if (MInput.Keyboard.Pressed(Keys.Q) && BuildingPipette.CanPick(template, GameplayScene.Instance.ProgressTree))
                    Pick(true, null);
            }
        }

        private void SyncDestructButton()
        {
            destructButton.GetComponent<ButtonScript>().IsSelected = TileLayerDestruction.IsMarked(tile, layer);
        }

        private void ToggleDestruction(bool value, ButtonScript sender)
        {
            TileLayerDestruction.Toggle(tile, layer);
        }

        private void Pick(bool value, ButtonScript sender)
        {
            BuildingPipette.Pick(template, Direction.DOWN);
        }
    }
}
