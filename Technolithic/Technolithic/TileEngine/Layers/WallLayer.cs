namespace Technolithic
{
    public class WallLayer : ITileLayer
    {
        public bool IsPresent(Tile tile)
        {
            return tile.WallId != -1;
        }

        public BuildingTemplate GetTemplate(Tile tile)
        {
            if (IsPresent(tile) == false)
                return null;

            return Engine.Instance.WallIdBuildingTemplate[tile.WallId];
        }

        public void Place(Tile tile, BuildingTemplate template)
        {
            tile.WallId = template.WallData.Id;
        }

        public void Remove(Tile tile)
        {
            tile.WallId = -1;
        }
    }
}
