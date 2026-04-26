namespace Technolithic
{
    public abstract class GameOverlay : IOverlay
    {
        public readonly GameOverlayType Type;
        public readonly MyTexture Icon;
        public readonly string Name;

        protected GameOverlay(GameOverlayType type, MyTexture icon, string name)
        {
            Type = type;
            Icon = icon;
            Name = name;
        }

        public abstract void Render(World world);
    }
}
