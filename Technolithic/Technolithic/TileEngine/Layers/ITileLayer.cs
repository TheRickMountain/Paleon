namespace Technolithic
{
    public interface ITileLayer
    {
        bool IsPresent(Tile tile);

        BuildingTemplate GetTemplate(Tile tile);

        void Place(Tile tile, BuildingTemplate template);

        void Remove(Tile tile);
    }
}
