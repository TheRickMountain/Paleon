using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldGenerator
    {
        private World world;
        private WorldManager worldManager;

        public WorldGenerator(World world, WorldManager worldManager)
        {
            this.world = world;
            this.worldManager = worldManager;

            int pointsAmount = 0;

            if(world.Width == 256)
            {
                pointsAmount = 32;
            }
            else if(world.Width == 128)
            {
                pointsAmount = 8;
            }

            foreach(string plantName in Engine.Instance.WorldGenerationData.Plants)
            {
                GeneratePlants(plantName, pointsAmount);
            }

            foreach (string treeName in Engine.Instance.WorldGenerationData.Trees)
            {
                GenerateTrees(treeName);
            }
        }

        private void GeneratePlants(string plantName, int pointsAmount)
        {
            int radius = 2;

            BuildingTemplate buildingTemplate = Engine.Instance.Buildings[plantName];

            Tile[] randomTiles = GetRandomTiles(pointsAmount);

            foreach (Tile centerTile in randomTiles)
            {
                foreach (Tile spawnTile in Utils.GetTilesInCircle(centerTile, radius))
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

    }
}
