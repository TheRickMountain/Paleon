using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class AnimalSpawnManager
    {

        private int minimalAnimalsCount = 20;

        private List<Tile> toSpawnTiles;

        public void Begin()
        {
            toSpawnTiles = new List<Tile>();

            for (int i = 0; i < GameplayScene.WorldSize; i++)
            {
                CheckAndAddTile(0, i);
                CheckAndAddTile(i, 0);
                CheckAndAddTile(GameplayScene.WorldSize - 1, i);
                CheckAndAddTile(i, GameplayScene.WorldSize - 1);
            }
        }

        private void CheckAndAddTile(int x, int y)
        {
            Tile tile = GameplayScene.Instance.World.GetTileAt(x, y);
            if (tile.GroundTopType != GroundTopType.Water && tile.GroundTopType != GroundTopType.DeepWater &&
                toSpawnTiles.Contains(tile) == false)
            {
                toSpawnTiles.Add(tile);
            }
        }

        public void NextHour(int hour)
        {
            // Если количество диких животных меньше минимальной, то через час рандомно спавнится животное
            if (GameplayScene.WorldManager.WildAnimalsNumber < minimalAnimalsCount && toSpawnTiles.Count > 0)
            {
                Tile tile = GetRandomSpawnTile();

                AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetRandomWildAnimalTemplate();

                int randomDaysUntilAging = MyRandom.Range(1, animalTemplate.DaysUntilAging);

                AnimalCmp animal = GameplayScene.Instance.SpawnAnimal(tile.X, tile.Y, animalTemplate, randomDaysUntilAging);

                if(animalTemplate.AnimalProduct != null)
                {
                    animal.ProductReadyPercent = MyRandom.Range(0, 100);
                }
            }
        }

        private Tile GetRandomSpawnTile()
        {
            return toSpawnTiles[MyRandom.Range(0, toSpawnTiles.Count)];
        }

        public void DebugRender()
        {
            for (int i = 0; i < toSpawnTiles.Count; i++)
            {
                Tile tile = toSpawnTiles[i];
                RenderManager.Rect(new Rectangle(tile.X * Engine.TILE_SIZE, tile.Y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE), Color.Orange * 0.5f);
            }
        }

    }
}