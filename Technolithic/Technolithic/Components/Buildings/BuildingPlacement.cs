using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class BuildingPlacement
    {
        public BuildingTemplate Template { get; }
        public Direction Direction { get; }

        public Tile[,] Tiles { get; private set; }
        public TileInfo[,] TilesInfosArray { get; }
        public List<TileInfo> TilesInfosList { get; }

        private readonly List<Tile> targetTiles = new List<Tile>();

        public BuildingPlacement(BuildingTemplate template, Direction direction)
        {
            Template = template;
            Direction = template.Rotatable ? direction : Direction.DOWN;

            TileInfo[,] tilesInfos = new TileInfo[template.Width, template.Height];
            TilesInfosList = new List<TileInfo>();

            for (int x = 0; x < tilesInfos.GetLength(0); x++)
            {
                for (int y = 0; y < tilesInfos.GetLength(1); y++)
                {
                    TileInfo tileInfo = new TileInfo(
                        template.WalkablePattern[x, y],
                        template.TargetPattern[x, y],
                        template.GroundPattern[x, y]);

                    tilesInfos[x, y] = tileInfo;
                    TilesInfosList.Add(tileInfo);
                }
            }

            TilesInfosArray = template.Rotatable ? Utils.RotateMatrix(tilesInfos, Direction) : tilesInfos;
        }

        public void Place(Tile[,] tiles, Entity entity)
        {
            Tiles = tiles;

            for (int column = 0; column < TilesInfosArray.GetLength(0); ++column)
            {
                for (int row = 0; row < TilesInfosArray.GetLength(1); ++row)
                {
                    TileInfo tileInfo = TilesInfosArray[column, row];

                    tileInfo.Tile = tiles[column, row];
                    tileInfo.Tile.Entity = entity;
                }
            }
        }

        public void Release()
        {
            foreach (var tileInfo in TilesInfosArray)
            {
                Tile tile = tileInfo.Tile;

                tile.Entity = null;
                tile.IsWalkable = true;

                if (Template.BuildingType == BuildingType.Wall)
                {
                    tile.StrengthValue = 0;
                }
            }
        }

        public void SetMarkType(MarkType markType)
        {
            foreach (var tileInfo in TilesInfosList)
            {
                tileInfo.Tile.MarkType = markType;
            }
        }

        public void UseNeighbourTilesAsTargets()
        {
            targetTiles.Clear();

            foreach (var tileInfo in TilesInfosArray)
            {
                foreach (var neighbourTile in tileInfo.Tile.GetNeighbourTiles())
                {
                    if (ContainsTile(neighbourTile) == false)
                        targetTiles.Add(neighbourTile);
                }
            }
        }

        public void UseTargetPatternAsTargets()
        {
            targetTiles.Clear();

            foreach (var tileInfo in TilesInfosArray)
            {
                if (tileInfo.IsTarget)
                    targetTiles.Add(tileInfo.Tile);
            }

            if (targetTiles.Count > 0)
                return;

            foreach (var tileInfo in TilesInfosArray)
            {
                foreach (var neighbourTile in tileInfo.Tile.GetNeighbourTiles())
                {
                    if (ContainsTile(neighbourTile) == false && targetTiles.Contains(neighbourTile) == false)
                        targetTiles.Add(neighbourTile);
                }
            }
        }

        public Tile GetReachableTile(CreatureCmp creature)
        {
            for (int i = 0; i < targetTiles.Count; ++i)
            {
                Tile tile = targetTiles[i];

                if (tile.IsWalkable && creature.Movement.CurrentTile.Room.Id == tile.Room.Id)
                    return tile;
            }

            return null;
        }

        public Tile GetReachableTile()
        {
            for (int i = 0; i < targetTiles.Count; ++i)
            {
                Tile tile = targetTiles[i];

                if (tile.IsWalkable)
                    return tile;
            }

            return null;
        }

        // TODO: переделать
        public Tile GetCenterTile()
        {
            return TilesInfosArray[0, 0].Tile;
        }

        public Room GetRoom()
        {
            return GetCenterTile().Room;
        }

        public Tile GetRealCenterTile()
        {
            switch (Direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    return TilesInfosArray[Template.Height / 2, Template.Width / 2].Tile;
                case Direction.UP:
                case Direction.DOWN:
                    return TilesInfosArray[Template.Width / 2, Template.Height / 2].Tile;
            }

            return null;
        }

        public bool IsCanBeAchieved()
        {
            Tile centerTile = GetCenterTile();

            Tile leftTile = centerTile.LeftTile;
            if (leftTile != null && leftTile.Room != null)
                return true;

            Tile rightTile = centerTile.RightTile;
            if (rightTile != null && rightTile.Room != null)
                return true;

            Tile topTile = centerTile.TopTile;
            if (topTile != null && topTile.Room != null)
                return true;

            Tile bottomTile = centerTile.BottomTile;
            if (bottomTile != null && bottomTile.Room != null)
                return true;

            return false;
        }

        public HashSet<Tile> GetTilesInRange(int range)
        {
            HashSet<Tile> tilesInRange = new HashSet<Tile>();

            foreach (var tileInfo in TilesInfosList)
            {
                Tile centerTile = tileInfo.Tile;

                centerTile.World.TryGetTilesInRadius(centerTile.X, centerTile.Y, range, tilesInRange);
            }

            foreach (var tileInfo in TilesInfosList)
            {
                tilesInRange.Remove(tileInfo.Tile);
            }

            return tilesInRange;
        }

        public Point[,] GetTilesAsPoints()
        {
            Point[,] points = new Point[Tiles.GetLength(0), Tiles.GetLength(1)];

            for (int column = 0; column < Tiles.GetLength(0); column++)
            {
                for (int row = 0; row < Tiles.GetLength(1); row++)
                {
                    points[column, row] = Tiles[column, row].GetAsPoint();
                }
            }

            return points;
        }

        private bool ContainsTile(Tile tile)
        {
            foreach (var tileInfo in TilesInfosList)
            {
                if (tileInfo.Tile == tile)
                    return true;
            }

            return false;
        }
    }
}
