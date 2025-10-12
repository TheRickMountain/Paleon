using Newtonsoft.Json;
using System.Collections.Generic;

namespace Technolithic
{
    public class ButcheringData
    {

        public IReadOnlyDictionary<string, int> RawLoot { get; init; } = new Dictionary<string, int>();
        public float ButcheringTimeInHours { get; init; }
        public float FreshnessTimeInHours { get; init; }

        [JsonIgnore]
        public IReadOnlyDictionary<Item, int> RealLoot { get; private set; }

        public void Initialize()
        {
            // TODO: в будущем, необходимо перенести этот функционал в ItemDatabase, чтобы он был универсальным
            Dictionary<Item, int> dict = new Dictionary<Item, int>();

            foreach (var kvp in RawLoot)
            {
                Item item = ItemDatabase.GetItemByName(kvp.Key);
                int count = kvp.Value;

                dict.Add(item, count);
            }

            RealLoot = dict;
        }

    }
}
