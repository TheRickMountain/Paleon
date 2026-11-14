using System;

namespace Technolithic
{
    public class TileLayerDestructFilter : DestructFilter
    {
        private readonly ITileLayer layer;
        private readonly string removalTemplateKey;
        private readonly Func<ProgressTree, bool> isAvailable;

        public TileLayerDestructFilter(string id, string nameKey, Func<MyTexture> getIcon, bool isEnabledByDefault,
            ITileLayer layer, string removalTemplateKey, Func<ProgressTree, bool> isAvailable = null)
            : base(id, nameKey, getIcon, isEnabledByDefault)
        {
            this.layer = layer;
            this.removalTemplateKey = removalTemplateKey;
            this.isAvailable = isAvailable;
        }

        public override bool IsAvailable(ProgressTree progressTree)
        {
            return isAvailable == null || isAvailable(progressTree);
        }

        public override bool Covers(Tile tile)
        {
            return layer.IsPresent(tile);
        }

        public override void MarkForDestruction(Tile tile)
        {
            if (Covers(tile) == false)
                return;

            BuildingTemplate removalTemplate = Engine.Instance.Buildings[removalTemplateKey];
            GameplayScene.WorldManager.TryToBuild(removalTemplate, tile.X, tile.Y, Direction.DOWN);
        }
    }
}
