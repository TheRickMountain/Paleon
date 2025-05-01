using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class TextureBank
    {

        public static MyTexture UITexture { get; private set; }
        public static MyTexture GroundTexture { get; private set; }
        public static MyTexture BlockTexture { get; private set; }

        public static Tileset GroundTileset { get; private set; }
        public static Tileset GroundTopTileset { get; private set; }
        public static Tileset SurfaceTileset { get; private set; }
        public static Tileset BlockTileset { get; private set; }
        public static Tileset UiTileset { get; private set; }

        public static void Initialize()
        {
            UITexture = ResourceManager.GetTexture("ui");
            GroundTexture = ResourceManager.GetTexture("tileset");
            BlockTexture = ResourceManager.GetTexture("block_tileset");

            GroundTileset = new Tileset(GroundTexture, 16, 16);
            GroundTopTileset = new Tileset(ResourceManager.GetTexture("ground_top"), 16, 16);
            SurfaceTileset = new Tileset(ResourceManager.GetTexture("surface_tileset"), 16, 16);
            BlockTileset = new Tileset(BlockTexture, 16, 16);
            UiTileset = new Tileset(UITexture, 16, 16);
        }

    }
}
