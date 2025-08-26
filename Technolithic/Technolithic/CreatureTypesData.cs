namespace Technolithic
{
    public enum CreatureType
    {
        Settler,
        Animal
    }

    public static class CreatureTypesData
    {

        public static string GetMultipleDisplayText(CreatureType creatureType) => creatureType switch
        {
            CreatureType.Settler => Localization.GetLocalizedText("settlers"),
            CreatureType.Animal => Localization.GetLocalizedText("animals"),
            _ => "None"
        };

    }
}
