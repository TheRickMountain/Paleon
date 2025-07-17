using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class World
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public PathTileGraph<Tile> PathTileGraph { get; private set; }

        private Tile[,] tiles;
        private MChunk[,] chunks;

        private TileMap summerGroundTileMap;
        private TileMap autumnGroundTileMap;
        private TileMap winterGroundTileMap;
        private TileMap springGroundTileMap;
        private TileMap groundTopTileMap;
        private TileMap surfaceTileMap;
        private TileMap blockTileMap;
        private TileMap itemTileMap;

        private Point tileMapCamMin = new Point();
        private Point tileMapCamMax = new Point();

        public World(int width, int height, WorldSaveData worldSaveData)
        {
            if (worldSaveData != null)
            {
                Width = worldSaveData.Width;
                Height = worldSaveData.Height;
            }
            else
            {
                Width = width;
                Height = height;
            }

            summerGroundTileMap = new TileMap(TextureBank.GroundTopTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            autumnGroundTileMap = new TileMap(TextureBank.GroundTopTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            winterGroundTileMap = new TileMap(TextureBank.GroundTopTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            springGroundTileMap = new TileMap(TextureBank.GroundTopTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            groundTopTileMap = new TileMap(TextureBank.GroundTopTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            surfaceTileMap = new TileMap(TextureBank.SurfaceTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Sides);
            blockTileMap = new TileMap(TextureBank.BlockTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);
            itemTileMap = new TileMap(TextureBank.GroundTileset, Engine.TILE_SIZE, width, height, TerrainConnection.Individual);

            CreateTiles();
            InitTilesNeighbours();

            PathTileGraph = new PathTileGraph<Tile>(tiles);

            PathAStar.Initialize(PathTileGraph);

            CreateChunks();
            InitChunksNeighbours();
        }

        public void Begin(WorldSaveData worldSaveData)
        {
            if (worldSaveData != null)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        TileSaveData tileSaveData = worldSaveData.Tiles[x, y];
                        Tile tile = tiles[x, y];

                        tile.GroundType = Utils.CheckAndGetCorrectEnum(tileSaveData.GroundType, GroundType.Ground);
                        tile.GroundTopType = Utils.CheckAndGetCorrectEnum(tileSaveData.GroundTopType, GroundTopType.None);
                        tile.SurfaceId = tileSaveData.SurfaceId;

                        tile.MoistureLevel = tileSaveData.MoistureLevel;
                        tile.FertilizerLevel = tileSaveData.FertilizerLevel;
                        tile.IrrigationStrength = tileSaveData.IrrigationStrength;

                        if (tileSaveData.InventoryItems != null)
                        {
                            foreach (var itemContainer in tileSaveData.InventoryItems)
                            {
                                Item item = ItemDatabase.GetItemById(itemContainer.Item1);
                                if (item != null)
                                {
                                    int factWeight = itemContainer.Item2;
                                    float durability = itemContainer.Item3;
                                    if (factWeight != 0)
                                    {
                                        tile.Inventory.AddCargo(new ItemContainer(item, factWeight, durability));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                HeightsGenerator waterGenerator = new HeightsGenerator(GameplayScene.WorldSize, GameplayScene.WorldSize,
                    MyRandom.Range(0, 999999), 7, 5, 0.3f);

                //Генерируем воду
                for (int x = 0; x < GameplayScene.WorldSize; x++)
                {
                    for (int y = 0; y < GameplayScene.WorldSize; y++)
                    {
                        Tile tile = GetTileAt(x, y);

                        float waterHeight = waterGenerator.GenerateHeight(x, y);
                        if (waterHeight > 1.6f)
                        {
                            tile.GroundTopType = GroundTopType.Water;
                        }
                    }
                }

                //Генерируем глубоководье
                for (int x = 0; x < GameplayScene.WorldSize; x++)
                {
                    for (int y = 0; y < GameplayScene.WorldSize; y++)
                    {
                        Tile tile = GetTileAt(x, y);

                        float waterHeight = waterGenerator.GenerateHeight(x, y);
                        if (waterHeight > 2.0f)
                        {
                            tile.GroundTopType = GroundTopType.DeepWater;
                        }
                    }
                }


                HeightsGenerator depositsGenerator = new HeightsGenerator(GameplayScene.WorldSize, GameplayScene.WorldSize, MyRandom.Range(1, 999999), 7, 5, 0.3f);

                HeightsGenerator clayGenerator = new HeightsGenerator(GameplayScene.WorldSize, GameplayScene.WorldSize, MyRandom.Range(1, 999999), 7, 3, 0.3f);
                HeightsGenerator woodGenerator = new HeightsGenerator(GameplayScene.WorldSize, GameplayScene.WorldSize, MyRandom.Range(1, 999999), 7, 3, 0.3f);
                HeightsGenerator stoneGenerator = new HeightsGenerator(GameplayScene.WorldSize, GameplayScene.WorldSize, MyRandom.Range(1, 999999), 7, 3, 0.3f);

                SpawnDeposits();

                // Генерация палок
                for (int x = 0; x < GameplayScene.WorldSize; x++)
                {
                    for (int y = 0; y < GameplayScene.WorldSize; y++)
                    {
                        if (depositsGenerator.GenerateHeight(x, y) < 0.25f)
                            continue;

                        Tile tile = GetTileAt(x, y);

                        float woodHeight = woodGenerator.GenerateHeight(x, y);
                        if (woodHeight > 2f && MyRandom.Range(2) == 1)
                        {
                            if (tile.GroundTopType != GroundTopType.Water && tile.GroundTopType != GroundTopType.DeepWater && tile.Entity == null)
                            {
                                GameplayScene.WorldManager.TryToBuild(Engine.Instance.Buildings["wood_pieces"], x, y, Direction.DOWN, true);
                                continue;
                            }
                        }
                    }
                }

                // Генерация камней
                for (int x = 0; x < GameplayScene.WorldSize; x++)
                {
                    for (int y = 0; y < GameplayScene.WorldSize; y++)
                    {
                        if (depositsGenerator.GenerateHeight(x, y) < 0.25f)
                            continue;

                        Tile tile = GetTileAt(x, y);

                        float stoneHeight = stoneGenerator.GenerateHeight(x, y);
                        if (stoneHeight > 2f && MyRandom.Range(2) == 1)
                        {
                            if (tile.GroundTopType != GroundTopType.Water && tile.GroundTopType != GroundTopType.DeepWater && tile.Entity == null)
                            {
                                GameplayScene.WorldManager.TryToBuild(Engine.Instance.Buildings["stone_pieces"], x, y, Direction.DOWN, true);
                                continue;
                            }
                        }
                    }
                }

                // Генерация травы
                for (int x = 0; x < GameplayScene.WorldSize; x++)
                {
                    for (int y = 0; y < GameplayScene.WorldSize; y++)
                    {
                        Tile tile = GetTileAt(x, y);
                        float clayHeight = clayGenerator.GenerateHeight(x, y);
                        if (tile.GroundTopType == GroundTopType.None && clayHeight < 2.5f)
                        {
                            tile.GroundType = GroundType.Grass;
                        }
                        else
                        {
                            tile.GroundType = GroundType.Ground;
                        }
                    }
                }
            }
        }

        private void SpawnDeposits()
        {
            // Делаем 10 попыток генерации жил
            Dictionary<GroundTopType, List<Tile>> depositsTiles = new Dictionary<GroundTopType, List<Tile>>();

            depositsTiles.Add(GroundTopType.Stone, new List<Tile>());
            depositsTiles.Add(GroundTopType.Clay, new List<Tile>());
            depositsTiles.Add(GroundTopType.Copper, new List<Tile>());
            depositsTiles.Add(GroundTopType.Iron, new List<Tile>());
            depositsTiles.Add(GroundTopType.Tin, new List<Tile>());

            for (int i = 0; i < 10; i++)
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Stone);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Stone].Add(tile);
                    break;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Clay);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Clay].Add(tile);
                    break;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Copper);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Copper].Add(tile);
                    break;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Iron);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Iron].Add(tile);
                    break;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Tin);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Tin].Add(tile);
                    break;
                }
            }

            // Дополнительная генерация жил
            if (MyRandom.ProbabilityChance(50))
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Stone);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Stone].Add(tile);
                }
            }

            if (MyRandom.ProbabilityChance(50))
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Clay);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Clay].Add(tile);
                }
            }

            if (MyRandom.ProbabilityChance(50))
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Copper);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Copper].Add(tile);
                }
            }

            if (MyRandom.ProbabilityChance(50))
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Iron);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Iron].Add(tile);
                }
            }

            if (MyRandom.ProbabilityChance(50))
            {
                Tile tile = TrySpawnDepositAtRandomPosition(GroundTopType.Tin);
                if (tile != null)
                {
                    depositsTiles[GroundTopType.Tin].Add(tile);
                }
            }

            // Генерация залежей вокруг жил
            foreach (var kvp in depositsTiles)
            {
                GroundTopType groundTopType = kvp.Key;

                foreach (var randomTile in kvp.Value)
                {
                    foreach (var tile in Utils.GetTilesInCircle(randomTile, 8))
                    {
                        if (MyRandom.ProbabilityChance(10))
                        {
                            string depositName = $"{groundTopType.ToString().ToLower()}_deposit";

                            BuildingTemplate depositBuildingTemplate;
                            if (Engine.Instance.Buildings.TryGetValue(depositName, out depositBuildingTemplate))
                            {
                                GameplayScene.WorldManager.TryToBuild(depositBuildingTemplate, tile.X, tile.Y, Direction.DOWN, true);
                            }
                            else
                            {
                                Program.WriteWarningLog($"The given key was not present in the dictionary: {depositName}");
                            }
                        }
                    }
                }
            }
        }

        private Tile TrySpawnDepositAtRandomPosition(GroundTopType groundTopType)
        {
            int randomX = MyRandom.Range(0, GameplayScene.WorldSize);
            int randomY = MyRandom.Range(0, GameplayScene.WorldSize);

            if (TrySpawnDeposit(randomX, randomY, groundTopType))
            {
                return GetTileAt(randomX, randomY);
            }
            else
            {
                return null;
            }
        }

        private bool TrySpawnDeposit(int x, int y, GroundTopType groundTopType)
        {
            for (int i = x - 1; i < x + 3; i++)
            {
                for (int j = y - 1; j < y + 3; j++)
                {
                    if (i < 0 || j < 0)
                        return false;

                    if (i >= GameplayScene.WorldSize || j >= GameplayScene.WorldSize)
                        return false;

                    Tile tile = GetTileAt(i, j);

                    if (tile.GroundTopType != GroundTopType.None)
                        return false;
                }
            }

            for (int i = x; i < x + 2; i++)
            {
                for (int j = y; j < y + 2; j++)
                {
                    Tile tile = GetTileAt(i, j);
                    tile.GroundTopType = groundTopType;
                }
            }

            return true;
        }

        public void Update()
        {
            Rectangle viewport = RenderManager.MainCamera.GetViewport();

            int xCamPoint = viewport.X / (Engine.TILE_SIZE * TileMap.CHUNK_SIZE);
            int yCamPoint = viewport.Y / (Engine.TILE_SIZE * TileMap.CHUNK_SIZE);

            int xViewPort = viewport.Right / (Engine.TILE_SIZE * TileMap.CHUNK_SIZE);
            int yViewPort = viewport.Bottom / (Engine.TILE_SIZE * TileMap.CHUNK_SIZE);

            int chunkColumns = Width / TileMap.CHUNK_SIZE;
            int chunkRows = Height / TileMap.CHUNK_SIZE;

            tileMapCamMin.X = Math.Max(0, xCamPoint);
            tileMapCamMin.Y = Math.Max(0, yCamPoint);
            tileMapCamMax.X = Math.Min(xViewPort + 1, chunkColumns);
            tileMapCamMax.Y = Math.Min(yViewPort + 1, chunkRows);
        }

        private void CreateTiles()
        {
            tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = new Tile(x, y, summerGroundTileMap, autumnGroundTileMap, winterGroundTileMap, springGroundTileMap, groundTopTileMap, surfaceTileMap, blockTileMap, itemTileMap);
                    tiles[x, y] = tile;
                }
            }
        }

        private void InitTilesNeighbours()
        {
            // Adding neighbours to tiles
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = tiles[x, y];

                    tile.LeftTile = GetTileAt(x - 1, y);
                    tile.RightTile = GetTileAt(x + 1, y);
                    tile.TopTile = GetTileAt(x, y - 1);
                    tile.BottomTile = GetTileAt(x, y + 1);

                    tile.LeftTopTile = GetTileAt(x - 1, y - 1);
                    tile.LeftBottomTile = GetTileAt(x - 1, y + 1);
                    tile.RightTopTile = GetTileAt(x + 1, y - 1);
                    tile.RightBottomTile = GetTileAt(x + 1, y + 1);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = tiles[x, y];
                    tile.GetNeighbourTiles();
                    tile.GetAllNeighbourTiles();
                }
            }
        }

        private void CreateChunks()
        {
            chunks = new MChunk[Width / MChunk.CHUNK_SIZE, Height / MChunk.CHUNK_SIZE];

            // Chunks creating
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    MChunk chunk = new MChunk(x, y);
                    chunks[x, y] = chunk;

                    int tX = x * MChunk.CHUNK_SIZE;
                    int tY = y * MChunk.CHUNK_SIZE;

                    for (int i = tX; i < tX + MChunk.CHUNK_SIZE; i++)
                    {
                        for (int j = tY; j < tY + MChunk.CHUNK_SIZE; j++)
                        {
                            chunk.AddTile(tiles[i, j]);
                        }
                    }
                }
            }
        }

        private void InitChunksNeighbours()
        {
            // Chunks neighbours setting
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    MChunk chunk = chunks[x, y];

                    if (x + 1 >= 0 && x + 1 < chunks.GetLength(0))
                        chunk.AddNeighbour(chunks[x + 1, y]);

                    if (y + 1 >= 0 && y + 1 < chunks.GetLength(1))
                        chunk.AddNeighbour(chunks[x, y + 1]);

                    if (x - 1 >= 0 && x - 1 < chunks.GetLength(0))
                        chunk.AddNeighbour(chunks[x - 1, y]);

                    if (y - 1 >= 0 && y - 1 < chunks.GetLength(1))
                        chunk.AddNeighbour(chunks[x, y - 1]);
                }
            }
        }

        public List<int> GetRoomsIds()
        {
            List<int> ids = new List<int>();

            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    MChunk chunk = chunks[x, y];

                    foreach (Room room in chunk.Rooms)
                    {
                        if (ids.Contains(room.ZoneId) == false)
                        {
                            ids.Add(room.ZoneId);
                        }
                    }
                }
            }

            return ids;
        }

        public void RenderUpdate()
        {
            summerGroundTileMap.RenderUpdate();
            autumnGroundTileMap.RenderUpdate();
            winterGroundTileMap.RenderUpdate();
            springGroundTileMap.RenderUpdate();
            groundTopTileMap.RenderUpdate();
            surfaceTileMap.RenderUpdate();
            blockTileMap.RenderUpdate();
            itemTileMap.RenderUpdate();
        }

        public void RenderSummerGroundTileMap(float alpha)
        {
            if (alpha > 0)
            {
                summerGroundTileMap.Render(tileMapCamMin, tileMapCamMax, Color.White * alpha);
            }
        }

        public void RenderAutumnGroundTileMap(float alpha)
        {
            if (alpha > 0)
            {
                autumnGroundTileMap.Render(tileMapCamMin, tileMapCamMax, Color.White * alpha);
            }
        }

        public void RenderWinterGroundTileMap(float alpha)
        {
            if (alpha > 0)
            {
                winterGroundTileMap.Render(tileMapCamMin, tileMapCamMax, Color.White * alpha);
            }
        }

        public void RenderSpringGroundTileMap(float alpha)
        {
            if (alpha > 0)
            {
                springGroundTileMap.Render(tileMapCamMin, tileMapCamMax, Color.White * alpha);
            }
        }

        public void RenderGroundTopTileMap()
        {
            groundTopTileMap.Render(tileMapCamMin, tileMapCamMax);
        }

        public void RenderSurfaceTileMap()
        {
            surfaceTileMap.Render(tileMapCamMin, tileMapCamMax);
        }

        public void RenderBlockTileMap()
        {
            blockTileMap.Render(tileMapCamMin, tileMapCamMax);
        }

        public void RenderItemTileMap()
        {
            itemTileMap.Render(tileMapCamMin, tileMapCamMax);
        }

        public Tile GetTileAt(int x, int y)
        {
            if (x < 0 || x >= Width)
                return null;

            if (y < 0 || y >= Height)
                return null;

            return tiles[x, y];
        }

        public Tile GetTileAt(Point point)
        {
            return GetTileAt(point.X, point.Y);
        }

        public Tile GetRandomTile()
        {
            int randomX = MyRandom.Range(0, Width);
            int randomY = MyRandom.Range(0, Height);

            return GetTileAt(randomX, randomY);
        }

        public WorldSaveData GetSaveData()
        {
            WorldSaveData worldSaveData = new WorldSaveData();
            worldSaveData.Width = Width;
            worldSaveData.Height = Height;
            worldSaveData.Tiles = new TileSaveData[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    worldSaveData.Tiles[x, y] = tiles[x, y].GetSaveData();
                }
            }

            return worldSaveData;
        }
    }
}