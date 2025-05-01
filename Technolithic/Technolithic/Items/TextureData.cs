using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TextureData
    {

        public string Source { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private MyTexture texture;

        public MyTexture Texture
        {
            get
            {
                if (texture == null)
                    texture = ResourceManager.GetTexture(Source).GetSubtexture(X, Y, Width, Height);

                return texture;
            }
        }


    }
}
