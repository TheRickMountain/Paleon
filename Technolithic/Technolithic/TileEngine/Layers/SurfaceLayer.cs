namespace Technolithic
{
    public class SurfaceLayer : ITileLayer
    {
        public bool IsPresent(Tile tile)
        {
            return tile.SurfaceId != -1;
        }

        public BuildingTemplate GetTemplate(Tile tile)
        {
            if (IsPresent(tile) == false)
                return null;

            return Engine.Instance.SurfaceIdBuildingTemplate[tile.SurfaceId];
        }

        public void Place(Tile tile, BuildingTemplate template)
        {
            tile.SurfaceId = template.SurfaceData.Id;
        }

        public void Remove(Tile tile)
        {
            tile.SurfaceId = -1;
        }
    }
}
