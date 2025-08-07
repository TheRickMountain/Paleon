using Newtonsoft.Json;
using System.Collections.Generic;

namespace Technolithic
{
    public class ButcheringData
    {
        public IReadOnlyDictionary<string, int> Loot { get; init; } = new Dictionary<string, int>();
        public float DurationInHours { get; init; }

        [JsonIgnore]
        public Dictionary<Item, int> RealLoot { get; private set; }

        public void Initialize()
        {
            RealLoot = ItemDatabase.ConvertRawLootToReal(Loot);
        }
    }
}
