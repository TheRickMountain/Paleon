using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WaterChunk
    {
        public Color Color { get; set; }
        
        public int MaxFishCount { get; private set; }
        public int CurrentFishCount { get; private set; }

        private List<Tile> tiles;

        private Timer timer;
        private int nextTime;

        private Fish fish;

        public bool IsUnrequired 
        {
            get
            {
                return tiles.Count <= 8;
            }
        }

        public WaterChunk()
        {
            tiles = new List<Tile>();
            timer = new Timer();
            nextTime = MyRandom.Range(75, 600);
        }

        private void CreateFish()
        {
            Tile randomTile = tiles[MyRandom.Range(0, tiles.Count)];
            fish = new Fish(new Vector2(randomTile.X * Engine.TILE_SIZE, randomTile.Y * Engine.TILE_SIZE));
            fish.CurrentTile = randomTile;
            fish.RestTime = MyRandom.Range(1, 5);
        }

        public void CatchFish()
        {
            CurrentFishCount--;
        }

        public void AddTile(Tile tile)
        {
            if (tiles.Contains(tile))
                return;

            tiles.Add(tile);
        }

        public void Initialize(int initFishesCount)
        {
            foreach (var tile in tiles)
            {
                tile.WaterChunk = this;
            }

            MaxFishCount = tiles.Count / 8;

            if (initFishesCount == -1)
            {
                CurrentFishCount = MaxFishCount;
            }
            else
            {
                CurrentFishCount = initFishesCount;
            }

            CreateFish();
        }

        public void Update()
        {
            // Прекращение спавна рыб в зимнее время
            if (CurrentFishCount < MaxFishCount && GameplayScene.Instance.WorldState.CurrentSeason != Season.Winter)
            {
                if (timer.GetTime() >= nextTime)
                {
                    timer.Reset();

                    CurrentFishCount++;

                    nextTime = MyRandom.Range(75, 600);
                }
            }

            if(CurrentFishCount > 0)
            {
                if (fish.TargetTile != null && fish.TargetTile != fish.CurrentTile)
                {
                    fish.Position = Vector2.Lerp(fish.CurrentTile.GetAsVector(), fish.TargetTile.GetAsVector(), fish.MovementProgress);
                    fish.Position *= Engine.TILE_SIZE;
                    if (fish.MovementProgress >= 1.0f)
                    {
                        fish.CurrentTile = fish.TargetTile;

                        fish.MovementProgress = 0;
                    }
                    else
                    {
                        fish.MovementProgress += Engine.GameDeltaTime * 0.3f;
                    }
                }
                else
                {
                    if (fish.RestTime <= 0)
                    {
                        List<Tile> waterNeighTiles = fish.CurrentTile.GetAllNeighbourTiles()
                            .Where(x => x.GroundTopType == GroundTopType.Water || x.GroundTopType == GroundTopType.DeepWater)
                            .Where(x => x.WaterChunk != null && x.WaterChunk == fish.CurrentTile.WaterChunk)
                            .ToList();

                        if (waterNeighTiles.Count == 0)
                        {
                            fish.TargetTile = fish.CurrentTile;
                        }
                        else
                        {
                            fish.TargetTile = waterNeighTiles[MyRandom.Range(0, waterNeighTiles.Count)];
                        }

                        fish.RestTime = MyRandom.Range(0, 5);

                        float rotation = Utils.Angle(fish.CurrentTile.GetAsVector(), fish.TargetTile.GetAsVector()) + 1.68f;
                        fish.Rotation = rotation;
                    }
                    else
                    {
                        fish.RestTime -= Engine.GameDeltaTime;
                    }
                }
            }
        }

        public void Render()
        {
            ResourceManager.FishTexture.Draw(fish.Position + new Vector2(8, 8), new Vector2(8, 8), Color.White * 0.2f, 1, fish.Rotation);
        }

        public void DebugRender()
        {
            foreach (var tile in tiles)
            {
                RenderManager.Rect(new Rectangle(tile.X * Engine.TILE_SIZE, tile.Y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE), Color);
            }
        }

        public WaterChunkSaveData GetSaveData()
        {
            var saveData = new WaterChunkSaveData();
            saveData.CurrentFishCount = CurrentFishCount;
            saveData.Tiles = new List<Tuple<int, int>>();
            foreach(var tile in tiles)
            {
                saveData.Tiles.Add(new Tuple<int, int>(tile.X, tile.Y));
            }

            return saveData;
        }

    }
}
