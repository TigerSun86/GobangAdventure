using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

public class AuraModifierConfig : ModifierConfig
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public string childModifierId;

    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public SkillTargetConfig skillTargetConfig;

    [JsonIgnore]
    public ModifierConfig childModifierConfig;

    // Hides the base class fields which are not used in this derived class
    [Obsolete("The field is not used.")]
    [JsonIgnore]
    public new float interval;

    [Obsolete("The field is not used.")]
    [JsonIgnore]
    public new Dictionary<ModifierPropertyType, float> properties;

    [Obsolete("The field is not used.")]
    [JsonIgnore]
    public new Dictionary<ModifierStateType, ModifierStateValue> states;

    public override ModifierConfig Clone()
    {
        AuraModifierConfig clone = (AuraModifierConfig)this.MemberwiseClone();
        return clone;
    }
}