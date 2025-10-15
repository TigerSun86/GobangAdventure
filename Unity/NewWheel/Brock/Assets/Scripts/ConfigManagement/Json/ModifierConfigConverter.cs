using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ModifierConfigConverter : JsonConverter<ModifierConfig>
{
    public override ModifierConfig ReadJson(JsonReader reader, Type objectType, ModifierConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string type = jsonObject["type"]?.Value<string>() ?? throw new JsonSerializationException("Effect missing 'type'.");

        ModifierConfig target = type switch
        {
            nameof(AuraModifierConfig) => new AuraModifierConfig(),
            nameof(ModifierConfig) => new ModifierConfig(),
            _ => throw new JsonSerializationException($"Unknown effect type: {type}")
        };

        serializer.Populate(jsonObject.CreateReader(), target);
        return target;
    }

    public override void WriteJson(JsonWriter writer, ModifierConfig value, JsonSerializer serializer)
        => serializer.Serialize(writer, value);
}
