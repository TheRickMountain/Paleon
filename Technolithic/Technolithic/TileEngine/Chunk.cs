using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Chunk
    {
        private Tileset tileset;

        public Tileset Tileset
        {
            get => tileset;
            set 
            { 
                tileset = value;

                isDirty = true;
            }
        }

        private int chunkSize;
        private int tileSize;

        private int[,] tiles;
        private Color[,] colors;
        
        private bool isDirty = true;

        private bool isVisible = false;

        private RenderTarget2D renderTarget;

        private Rectangle destRect;

        public Chunk(int x, int y, int chunkSize, int tileSize)
        {
            this.chunkSize = chunkSize;
            this.tileSize = tileSize;

            tiles = new int[chunkSize, chunkSize];
            colors = new Color[chunkSize, chunkSize];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = -1;
                    colors[i, j] = Color.White;
                }
            }

            int renderTargetSize = tileSize * chunkSize;

            renderTarget = new RenderTarget2D(
                Engine.Instance.GraphicsDevice,
                renderTargetSize,
                renderTargetSize,
                false,
                Engine.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            destRect = new Rectangle(x, y, renderTargetSize, renderTargetSize);

            isDirty = true;
        }

        public void RenderUpdate()
        {
            if (isDirty)
            {
                // If chunk doesn't has any tile it will be invisible
                isVisible = HasAnyVisibleTile();

                DrawSceneToTexture(renderTarget);
                isDirty = false;
            }
        }

        public void Render()
        {
            if (isVisible)
            {
                RenderManager.SpriteBatch.Draw(renderTarget, destRect, Color.White);
            }
        }

        public void Render(Color color)
        {
            if (isVisible)
            {
                RenderManager.SpriteBatch.Draw(renderTarget, destRect, color);
            }
        }

        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target
            Engine.Instance.GraphicsDevice.SetRenderTarget(renderTarget);

            // Draw the scene
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    int tileId = tiles[x, y];

                    if (tileId == -1) continue;

                    Tileset?[tileId].Draw(new Vector2(x * tileSize, y * tileSize), Vector2.Zero, colors[x, y]);
                }
            }

            RenderManager.SpriteBatch.End();

            // Drop the render target
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        public void SetCell(int x, int y, int tileId, Color color)
        {
            if (tiles[x, y] == tileId && colors[x, y] == color)
                return;

            tiles[x, y] = tileId;
            colors[x, y] = color;

            isDirty = true;
        }

        public int GetCell(int x, int y)
        {
            return tiles[x, y];
        }

        private bool HasAnyVisibleTile()
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (tiles[x, y] != -1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
