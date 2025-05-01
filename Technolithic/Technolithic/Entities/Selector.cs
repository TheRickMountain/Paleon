using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EntitySelector
    {
        private MyTexture leftTopCorner;
        private MyTexture rightTopCorner;
        private MyTexture leftDownCorner;
        private MyTexture rightDownCorner;

       
        public EntitySelector()
        {
            leftTopCorner = TextureBank.UITexture.GetSubtexture(80, 48, 7, 7);
            rightTopCorner = TextureBank.UITexture.GetSubtexture(87, 48, 7, 7);
            leftDownCorner = TextureBank.UITexture.GetSubtexture(80, 55, 7, 7);
            rightDownCorner = TextureBank.UITexture.GetSubtexture(87, 55, 7, 7);
        }

        public void Render(int x, int y, int width, int height, Color color)
        {
            leftTopCorner.Draw(new Rectangle(x - 3, y - 3, 7, 7), color);
            rightTopCorner.Draw(new Rectangle(x + width - 4, y - 3, 7, 7), color);
            leftDownCorner.Draw(new Rectangle(x - 3, y + height - 4, 7, 7), color);
            rightDownCorner.Draw(new Rectangle(x + width - 4, y + height - 4, 7, 7), color);
        }

        public void Render(Rectangle rect, Color color)
        {
            leftTopCorner.Draw(new Rectangle(rect.X - 3, rect.Y - 3, 7, 7), color);
            rightTopCorner.Draw(new Rectangle(rect.X + rect.Width - 4, rect.Y - 3, 7, 7), color);
            leftDownCorner.Draw(new Rectangle(rect.X - 3, rect.Y + rect.Height - 4, 7, 7), color);
            rightDownCorner.Draw(new Rectangle(rect.X + rect.Width - 4, rect.Y + rect.Height - 4, 7, 7), color);
        }

    }
}
