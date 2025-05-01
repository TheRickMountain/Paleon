using Newtonsoft.Json;
using System;

namespace Technolithic
{
    public class AnimalTemplateJsonConverter : JsonConverter<AnimalTemplate>
    {

        public override AnimalTemplate ReadJson(JsonReader reader, Type objectType, AnimalTemplate existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return AnimalTemplateDatabase.GetAnimalTemplateByName(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, AnimalTemplate value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}