using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class IrrigationOverlay : IOverlay
    {
        public void Render(World world)
        {
            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    if (world.GetTileAt(x, y).IrrigationLevel > 0)
                    {
                        RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, Color.Blue * 0.5f);
                    }
                }
            }
        }
    }
}
