using Newtonsoft.Json;
using System;

namespace Technolithic
{
    public class ItemJsonConverter : JsonConverter<Item>
    {

        public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return ItemDatabase.GetItemByName(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}