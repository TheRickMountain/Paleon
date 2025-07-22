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

        public static Tileset SpringGroundTileset { get; private set; }
        public static Tileset SummerGroundTileset { get; private set; }
        public static Tileset AutumnGroundTileset { get; private set; }
        public static Tileset WinterGroundTileset { get; private set; }

        public static Tileset GroundTopTileset { get; private set; }
        public static Tileset SurfaceTileset { get; private set; }
        public static Tileset BlockTileset { get; private set; }
        public static Tileset ItemTileset { get; private set; }

        public static void Initialize()
        {
            UITexture = ResourceManager.GetTexture("ui");

            SpringGroundTileset = new Tileset(ResourceManager.GetTexture("spring_ground_tileset"), 16, 16);
            SummerGroundTileset = new Tileset(ResourceManager.GetTexture("summer_ground_tileset"), 16, 16);
            AutumnGroundTileset = new Tileset(ResourceManager.GetTexture("autumn_ground_tileset"), 16, 16);
            WinterGroundTileset = new Tileset(ResourceManager.GetTexture("winter_ground_tileset"), 16, 16);

            GroundTopTileset = new Tileset(ResourceManager.GetTexture("ground_top"), 16, 16);
            SurfaceTileset = new Tileset(ResourceManager.GetTexture("surface_tileset"), 16, 16);
            BlockTileset = new Tileset(ResourceManager.GetTexture("block_tileset"), 16, 16);
            ItemTileset = new Tileset(ResourceManager.GetTexture("tileset"), 16, 16);
        }

    }
}
