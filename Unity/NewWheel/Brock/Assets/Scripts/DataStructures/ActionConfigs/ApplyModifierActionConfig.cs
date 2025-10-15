using Newtonsoft.Json;

public class ApplyModifierActionConfig : ActionConfig
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public string modifierId;

    [JsonIgnore]
    public ModifierConfig modifierConfig;
}