namespace Technolithic
{
    public class TBigButtonUI : TButtonUI
    {
        public TBigButtonUI(Scene scene) : base(scene)
        {
            ButtonTexture = TextureBank.UITexture.GetSubtexture(0, 64, 24, 24);
            Width = 48;
            Height = 48;
        }
    }
}
