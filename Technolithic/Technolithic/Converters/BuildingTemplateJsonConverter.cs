using Newtonsoft.Json;
using System;

namespace Technolithic
{
    public class BuildingTemplateJsonConverter : JsonConverter<BuildingTemplate>
    {

        public override BuildingTemplate ReadJson(JsonReader reader, Type objectType, BuildingTemplate existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return Engine.Instance.Buildings[reader.Value.ToString()];
        }

        public override void WriteJson(JsonWriter writer, BuildingTemplate value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}