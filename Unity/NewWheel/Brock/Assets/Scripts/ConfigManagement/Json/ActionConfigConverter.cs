using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ActionConfigConverter : JsonConverter<ActionConfig>
{
    public override ActionConfig ReadJson(JsonReader reader, Type objectType, ActionConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string type = jsonObject["type"]?.Value<string>() ?? throw new JsonSerializationException("Effect missing 'type'.");

        ActionConfig target = type switch
        {
            nameof(DamageActionConfig) => new DamageActionConfig(),
            nameof(ApplyAuraModifierActionConfig) => new ApplyAuraModifierActionConfig(),
            nameof(ApplyModifierActionConfig) => new ApplyModifierActionConfig(),
            nameof(RemoveModifierActionConfig) => new RemoveModifierActionConfig(),
            nameof(LifestealActionConfig) => new LifestealActionConfig(),
            nameof(StunActionConfig) => new StunActionConfig(),
            nameof(LinearProjectileActionConfig) => new LinearProjectileActionConfig(),
            nameof(HealActionConfig) => new HealActionConfig(),
            nameof(ReviveActionConfig) => new ReviveActionConfig(),
            _ => throw new JsonSerializationException($"Unknown effect type: {type}")
        };

        serializer.Populate(jsonObject.CreateReader(), target);
        return target;
    }

    public override void WriteJson(JsonWriter writer, ActionConfig value, JsonSerializer serializer)
        => serializer.Serialize(writer, value);
}
