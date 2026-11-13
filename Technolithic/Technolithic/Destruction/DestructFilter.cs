using System;

namespace Technolithic
{
    public abstract class DestructFilter
    {
        public string Id { get; }
        public bool IsEnabledByDefault { get; }

        public string Name => Localization.GetLocalizedText(nameKey);
        public MyTexture Icon => getIcon();

        private readonly string nameKey;
        private readonly Func<MyTexture> getIcon;

        protected DestructFilter(string id, string nameKey, Func<MyTexture> getIcon, bool isEnabledByDefault)
        {
            Id = id;
            IsEnabledByDefault = isEnabledByDefault;

            this.nameKey = nameKey;
            this.getIcon = getIcon;
        }

        public abstract bool Covers(Tile tile);

        public abstract void MarkForDestruction(Tile tile);
    }
}
