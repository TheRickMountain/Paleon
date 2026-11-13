using System.Linq;

namespace Technolithic
{
    public class IrrigationCanalLayer : ITileLayer
    {
        private BuildingTemplate canalTemplate;

        public bool IsPresent(Tile tile)
        {
            return tile.GroundTopType == GroundTopType.IrrigationCanalEmpty
                || tile.GroundTopType == GroundTopType.IrrigationCanalFull;
        }

        public BuildingTemplate GetTemplate(Tile tile)
        {
            if (IsPresent(tile) == false)
                return null;

            return canalTemplate ??= Engine.Instance.Buildings.Values
                .FirstOrDefault(template => template.BuildingType == BuildingType.BuildIrrigationCanal);
        }

        public void Place(Tile tile, BuildingTemplate template)
        {
            if (tile.GroundType == GroundType.Grass)
            {
                tile.GroundType = GroundType.Ground;
            }

            GameplayScene.Instance.BuildIrrigationCanal(tile);
        }

        public void Remove(Tile tile)
        {
            GameplayScene.Instance.DestructIrrigationCanal(tile);
        }
    }
}
