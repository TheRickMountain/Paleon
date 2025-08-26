namespace Technolithic
{
    public enum GameActionCategory
    {
        CollectionAndExtractionOfResources,
        AnimalHusbandryAndHunting,
        Agriculture,
        Destruct,
        Cancel
    }


    public static class GameActionCategoryDatabase
    {
        public static void Initialize()
        {
            EnumDatabase<GameActionCategory>.Register(
                GameActionCategory.CollectionAndExtractionOfResources,
                TextureBank.UITexture.GetSubtexture(192, 128, 16, 16),
                "collection_and_extraction_of_resources");
            
            EnumDatabase<GameActionCategory>.Register(
                GameActionCategory.Agriculture,
                TextureBank.UITexture.GetSubtexture(208, 160, 16, 16), 
                "agriculture");

            EnumDatabase<GameActionCategory>.Register(
                GameActionCategory.AnimalHusbandryAndHunting,
                TextureBank.UITexture.GetSubtexture(176, 112, 16, 16),
                "animal_husbandry_and_hunting");

            EnumDatabase<GameActionCategory>.Register(
                GameActionCategory.Destruct, 
                TextureBank.UITexture.GetSubtexture(0, 16, 16, 16), 
                "destruct");
            
            EnumDatabase<GameActionCategory>.Register(
                GameActionCategory.Cancel, 
                TextureBank.UITexture.GetSubtexture(48, 0, 16, 16), 
                "cancel");
        }
    }
}