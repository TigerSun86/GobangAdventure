using Newtonsoft.Json;

public class RemoveModifierActionConfig : ActionConfig
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public string modifierId;

    [JsonIgnore]
    public ModifierConfig modifierConfig;
}