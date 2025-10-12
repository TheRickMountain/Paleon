namespace Technolithic
{
    public class AnimalCorpse : Entity
    {

        public AnimalCorpse(AnimalTemplate animalTemplate)
        {
            Add(new AnimalCorpseCmp(animalTemplate));

            // TODO: Получать размеры из AnimalTemplate
            Add(new SelectableCmp(0, 0, 24, 24, SelectableType.AnimalCorpse));
        }

    }
}
