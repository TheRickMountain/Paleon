namespace Technolithic
{
    public class FarmlandLayer : ITileLayer
    {
        private const string FarmlandTemplateName = "farmland";

        public bool IsPresent(Tile tile)
        {
            return tile.IsFarmland;
        }

        public BuildingTemplate GetTemplate(Tile tile)
        {
            if (IsPresent(tile) == false)
                return null;

            return Engine.Instance.Buildings[FarmlandTemplateName];
        }

        public void Place(Tile tile, BuildingTemplate template)
        {
            tile.GroundType = GroundType.FarmPlot;
            tile.UpdateFarmlandView();
        }

        public void Remove(Tile tile)
        {
            tile.GroundType = GroundType.Ground;
            tile.IrrigateFarmland = false;
            tile.FertilizeFarmland = false;
        }
    }
}
