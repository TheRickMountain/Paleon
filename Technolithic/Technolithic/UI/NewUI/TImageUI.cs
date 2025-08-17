using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Technolithic
{
    public class TImageUI : MNode
    {
        public MyTexture Texture { get; set; } = RenderManager.Pixel;

        public TImageUI(Scene scene) : base(scene)
        {
            
        }

        public override void Render()
        {
            if (Texture == null)
            {
                base.Render();
                return;
            }

            Vector2 textureSize = Texture.Size;

            if (textureSize.X == 0 || textureSize.Y == 0)
            {
                base.Render();
                return;
            }

            float ratioX = Width / textureSize.X;
            float ratioY = Height / textureSize.Y;

            Vector2 sizeRatio = new Vector2(ratioX, ratioY);

            Texture.Draw(new Vector2(X, Y), Vector2.Zero / sizeRatio, SelfColor, Vector2.One * sizeRatio, 0, SpriteEffects.None);

            base.Render();
        }

    }
}
