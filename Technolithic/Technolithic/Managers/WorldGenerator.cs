using System.Collections.Generic;

namespace Technolithic
{
    public class WorldGenerator
    {
        private World world;
        private WorldManager worldManager;
        private readonly WorldSettings _worldSettings;
        private readonly FastNoiseLite _noise;

        public WorldGenerator(World world, WorldManager worldManager, WorldSettings worldSettings)
        {
            this.world = world;
            this.worldManager = worldManager;
            _worldSettings = worldSettings;
            _noise = new FastNoiseLite(worldSettings.Seed);
        }

        public void GenerateWorld()
        {
            GenerateWater();

            GenerateMixed();

            int pointsAmount = 0;

            if (world.Width == 256)
            {
                pointsAmount = 32;
            }
            else if (world.Width == 128)
            {
                pointsAmount = 8;
            }

            foreach (string plantName in Engine.Instance.WorldGenerationData.Plants)
            {
                GeneratePlants(plantName, pointsAmount);
            }

            foreach (string treeName in Engine.Instance.WorldGenerationData.Trees)
            {
                GenerateTrees(treeName);
            }
        }

        private void GenerateWater()
        {
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise.SetFrequency(0.01f);

            for (int x = 0; x < _worldSettings.Size; x++)
            {
                for (int y = 0; y < _worldSettings.Size; y++)
                {
                    Tile tile = world.GetTileAt(x, y);

                    float height = _noise.GetNoise(x, y);

                    if(height < -0.23f)
                    {
                        tile.GroundTopType = GroundTopType.Water;
                    }
                }
            }

            for (int x = 0; x < _worldSettings.Size; x++)
            {
                for (int y = 0; y < _worldSettings.Size; y++)
                {
                    Tile tile = world.GetTileAt(x, y);

                    float height = _noise.GetNoise(x, y);

                    if (height < -0.26f)
                    {
                        tile.GroundTopType = GroundTopType.DeepWater;
                    }
                }
            }
        }

        private void GenerateMixed()
        {
            // TODO: заменить на FastNoise
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

                    Tile tile = world.GetTileAt(x, y);

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

                    Tile tile = world.GetTileAt(x, y);

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
                    Tile tile = world.GetTileAt(x, y);
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

        private void GeneratePlants(string plantName, int pointsAmount)
        {
            int radius = 2;

            BuildingTemplate buildingTemplate = Engine.Instance.Buildings[plantName];

            Tile[] randomTiles = GetRandomTiles(pointsAmount);

            foreach (Tile centerTile in randomTiles)
            {
                foreach (Tile spawnTile in world.GetTilesWithinRadius(centerTile, radius))
                {
                    if (MyRandom.ProbabilityChance(50)) continue;

                    Entity entity = worldManager.TryToBuild(buildingTemplate, spawnTile.X, spawnTile.Y, Direction.DOWN, true);
                    if (entity != null)
                    {
                        FarmPlot wildFarmPlot = entity.Get<FarmPlot>();
                        wildFarmPlot.SetPlantParameters(MyRandom.Range(3) == 2 ? 100 : MyRandom.Range(75, 100), 0);
                        wildFarmPlot.MakeWild();
                    }
                }
            }
        }

        private void GenerateTrees(string plantName)
        {
            BuildingTemplate buildingTemplate = Engine.Instance.Buildings[plantName];

            CellularAutomata oakCellularAutomata = new CellularAutomata(world.Width, new List<int>() { 5, 6, 7, 8 }, new List<int>() { 4, 5, 6, 7, 8 });
            oakCellularAutomata.GenerateRandomMap();
            oakCellularAutomata.Update(21);

            oakCellularAutomata.ZoomIn(32, 32, 64);
            oakCellularAutomata.Update(6);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    if (oakCellularAutomata.GetCell(x, y) == 1 && MyRandom.Range(1, 6 + 1) == 1)
                    {
                        Entity entity = worldManager.TryToBuild(buildingTemplate, x, y, Direction.DOWN, true);
                        if (entity != null)
                        {
                            TreeBuilding treeBuilding = entity.Get<TreeBuilding>();
                            treeBuilding.SetGrowthProgress(MyRandom.FromSet(0.1f, 0.2f, 0.5f, 0.8f, 0.8f, 1.0f, 1.0f, 1.0f));
                            continue;
                        }
                    }
                }
            }
        }

        private Tile[] GetRandomTiles(int amount)
        {
            Tile[] tiles = new Tile[amount];

            for (int i = 0; i < amount; i++)
            {
                tiles[i] = world.GetRandomTile();
            }

            return tiles;
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
                    foreach (var tile in world.GetTilesWithinRadius(randomTile, 8))
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
                return world.GetTileAt(randomX, randomY);
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

                    Tile tile = world.GetTileAt(i, j);

                    if (tile.GroundTopType != GroundTopType.None)
                        return false;
                }
            }

            for (int i = x; i < x + 2; i++)
            {
                for (int j = y; j < y + 2; j++)
                {
                    Tile tile = world.GetTileAt(i, j);
                    tile.GroundTopType = groundTopType;
                }
            }

            return true;
        }
    }
}
