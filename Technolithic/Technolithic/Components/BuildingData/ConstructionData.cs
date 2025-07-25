using Newtonsoft.Json;
using System.Collections.Generic;

namespace Technolithic
{
    public class ConstructionData
    {

        public LaborType LaborType { get; init; }
        public InteractionType InteractionType { get; init; }
        public float DurationInHours { get; init; }
        public ToolUsageStatus ToolUsageStatus { get; init; }
        public IReadOnlyDictionary<string, int> Ingredients { get; init; } = new Dictionary<string, int>();

        [JsonIgnore]
        public IReadOnlyDictionary<Item, int> RealIngredients { get; private set; }

        public void Initialize()
        {
            RealIngredients = ItemDatabase.ConvertRawLootToReal(Ingredients);
        }

    }
}
