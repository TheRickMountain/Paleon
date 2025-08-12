namespace Technolithic
{
    public static class CreatureTypesData
    {

        public static string GetCreatureTypeMultipleDisplayText(CreatureType creatureType) => creatureType switch
        {
            CreatureType.Settler => Localization.GetLocalizedText("settlers"),
            CreatureType.Animal => Localization.GetLocalizedText("animals"),
            _ => "None"
        };

    }
}
