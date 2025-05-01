using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum TerrainConnection
    {
        Individual,
        Sides
    }

    public class TileMap
    {
        public Tileset Tileset { get; private set; }

        public int TileSize { get; private set; }

        public const int CHUNK_SIZE = 16;

        public int TileColumns { get; private set; }
        public int TileRows { get; private set; }

        private TerrainConnection terrainConnection;

        private int chunkColumns;
        private int chunkRows;

        private Chunk[,] chunks;

        private int[,] terrainIdGrid;

        public TileMap(Tileset tileset, int tileSize, int width, int height, TerrainConnection terrainConnection)
        {
            Tileset = tileset;

            TileSize = tileSize;

            TileColumns = width;
            TileRows = height;

            this.terrainConnection = terrainConnection;

            CreateChunks();

            terrainIdGrid = new int[TileColumns, TileRows];

            for (int x = 0; x < TileColumns; x++)
            {
                for (int y = 0; y < TileRows; y++)
                {
                    terrainIdGrid[x, y] = -1;
                }
            }
        }

        private void CreateChunks()
        {
            chunkColumns = TileColumns / CHUNK_SIZE;
            chunkRows = TileRows / CHUNK_SIZE;

            chunks = new Chunk[chunkColumns, chunkRows];

            for (int x = 0; x < chunkColumns; x++)
            {
                for (int y = 0; y < chunkRows; y++)
                {
                    int chunkX = x * CHUNK_SIZE * TileSize;
                    int chunkY = y * CHUNK_SIZE * TileSize;

                    chunks[x, y] = new Chunk(chunkX, chunkY, CHUNK_SIZE, TileSize);
                    chunks[x, y].Tileset = Tileset;
                }
            }
        }

        public void RenderUpdate()
        {
            for (int x = 0; x < chunkColumns; x++)
                for (int y = 0; y < chunkRows; y++)
                    chunks[x, y].RenderUpdate();
        }

        public void Render(Point camMin, Point camMax)
        {
            for (int x = camMin.X; x < camMax.X; x++)
                for (int y = camMin.Y; y < camMax.Y; y++)
                    chunks[x, y].Render();
        }

        public void Render(Point camMin, Point camMax, Color color)
        {
            for (int x = camMin.X; x < camMax.X; x++)
                for (int y = camMin.Y; y < camMax.Y; y++)
                    chunks[x, y].Render(color);
        }

        public void SetCell(int x, int y, int tile)
        {
            SetCell(x, y, tile, Color.White);
        }

        public void SetCell(int x, int y, int tile, Color color)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return;

            int chunkX = x / CHUNK_SIZE;
            int chunkY = y / CHUNK_SIZE;

            chunks[chunkX, chunkY].SetCell(x - chunkX * CHUNK_SIZE, y - chunkY * CHUNK_SIZE, tile, color);
        }

        private void SetCellTileId(int x, int y, int tileId)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return;

            int chunkX = x / CHUNK_SIZE;
            int chunkY = y / CHUNK_SIZE;

            chunks[chunkX, chunkY].SetCell(x - chunkX * CHUNK_SIZE, y - chunkY * CHUNK_SIZE, tileId, Color.White);
        }

        public void SetCellTerrainId(int x, int y, int terrainId)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return;

            if (terrainIdGrid[x, y] == terrainId)
                return;

            terrainIdGrid[x, y] = terrainId;

            switch(terrainConnection)
            {
                case TerrainConnection.Individual:
                    {
                        SetCellTileId(x, y, terrainId);
                    }
                    break;
                case TerrainConnection.Sides:
                    {
                        UpdateCellTerrain(x, y);

                        UpdateCellTerrain(x - 1, y);
                        UpdateCellTerrain(x + 1, y);
                        UpdateCellTerrain(x, y - 1);
                        UpdateCellTerrain(x, y + 1);
                    }
                    break;
            }
        }

        public int GetCellTerrainId(int x, int y)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return -1;

            return terrainIdGrid[x, y];
        }

        private void UpdateCellTerrain(int x, int y)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return;

            int terrainId = terrainIdGrid[x, y];

            if(terrainId == -1)
            {
                SetCellTileId(x, y, -1);
                return;
            }

            switch(terrainConnection)
            {
                case TerrainConnection.Sides:
                    {
                        bool top = GetCellTerrainId(x, y - 1) == terrainId;
                        bool left = GetCellTerrainId(x - 1, y) == terrainId;
                        bool right = GetCellTerrainId(x + 1, y) == terrainId;
                        bool bottom = GetCellTerrainId(x, y + 1) == terrainId;

                        int regionTileId = NewBitmaskGenerator.Get4BitBitmask(top, left, right, bottom);

                        int globalTileId = terrainId * 16 + regionTileId;

                        SetCellTileId(x, y, globalTileId);
                    }
                    break;
            }
        }

    }
}
