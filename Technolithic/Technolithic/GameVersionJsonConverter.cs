using Newtonsoft.Json;
using System;

namespace Technolithic
{
    public class GameVersionJsonConverter : JsonConverter<GameVersion>
    {

        public override GameVersion ReadJson(JsonReader reader, Type objectType, GameVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string versionString = (string)reader.Value;
                string[] parts = versionString.Split('.');
                if (parts.Length == 3 && int.TryParse(parts[0], out int major) &&
                    int.TryParse(parts[1], out int minor) && int.TryParse(parts[2], out int patch))
                {
                    return new GameVersion(major, minor, patch);
                }
            }
            throw new JsonSerializationException("Invalid version format");
        }

        public override void WriteJson(JsonWriter writer, GameVersion value, JsonSerializer serializer)
        {
            GameVersion version = value;
            writer.WriteValue(version.ToString());
        }
    }
}
