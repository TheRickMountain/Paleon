using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WaterChunkManager
    {
        public List<WaterChunk> WaterChunks { get; private set; }

        public WaterChunkManager()
        {
            WaterChunks = new List<WaterChunk>();
        }

        public void AddWaterChunk(WaterChunk waterChunk)
        {
            if (waterChunk == null)
                throw new ArgumentNullException();

            if (WaterChunks.Contains(waterChunk))
                return;

            WaterChunks.Add(waterChunk);
        }

        public void Update()
        {
            for (int i = 0; i < WaterChunks.Count; i++)
            {
                WaterChunks[i].Update();
            }
        }

        public void Render()
        {
            for (int i = 0; i < WaterChunks.Count; i++)
            {
                WaterChunks[i].Render();
            }
        }

        public void DebugRender()
        {
            for (int i = 0; i < WaterChunks.Count; i++)
            {
                WaterChunks[i].DebugRender();
            }
        }

    }
}
