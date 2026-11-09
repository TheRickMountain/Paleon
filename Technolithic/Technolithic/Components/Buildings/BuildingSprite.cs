namespace Technolithic
{
    public static class BuildingSprite
    {
        public static AnimatedSprite Create(BuildingTemplate template, Direction direction)
        {
            if (template.BuildingType == BuildingType.Wall)
                return null;

            MyTexture texture = template.Textures[direction];

            int textureWidth = -1;
            int textureHeight = -1;

            switch (direction)
            {
                case Direction.DOWN:
                case Direction.UP:
                    textureWidth = template.TextureWidth;
                    textureHeight = template.TextureHeight;
                    break;
                case Direction.LEFT:
                case Direction.RIGHT:
                    textureWidth = template.TextureHeight;
                    textureHeight = template.TextureWidth;
                    break;
            }

            AnimatedSprite sprite = new AnimatedSprite(textureWidth, textureHeight);
            sprite.Add("Idle", new Animation(texture, 1, 0, textureWidth, textureHeight, 0, 0, 1));

            if (template.Animated)
            {
                int frameCount = texture.Width / textureWidth;
                sprite.Add("Process", new Animation(texture, frameCount, 0, textureWidth, textureHeight, 0, textureHeight, 5));
            }

            return sprite;
        }

        public static void AttachTo(AnimatedSprite sprite, Entity entity, BuildingPlacement placement)
        {
            sprite.Entity = entity;

            sprite.SetOrigin(sprite.Width / 2, sprite.Height / 2);

            int imageYOffset = placement.TilesInfosArray.GetLength(1) * Engine.TILE_SIZE - sprite.Height;
            int imageXOffset = (placement.TilesInfosArray.GetLength(0) * Engine.TILE_SIZE - sprite.Width) / 2;

            sprite.X = imageXOffset + sprite.Width / 2;
            sprite.Y = imageYOffset + sprite.Height / 2;

            sprite.Play("Idle");
        }
    }
}
