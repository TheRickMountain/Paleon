using Penumbra;

namespace Technolithic
{
    public class AnimalCorpse : Entity
    {

        public AnimalCorpse(AnimalTemplate animalTemplate, Tile tile, InteractablesManager interactablesManager)
        {
            Add(new AnimalCorpseCmp(animalTemplate, tile, interactablesManager));

            int textureWidth = animalTemplate.DeadTexture.Width;
            int textureHeight = animalTemplate.DeadTexture.Height;

            Add(new SelectableCmp(
                (Engine.TILE_SIZE / 2) - (textureWidth / 2),
                -(textureHeight - Engine.TILE_SIZE),
                textureWidth, textureHeight, SelectableType.AnimalCorpse));
        }

    }
}
