using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

public class ModifierConfig
{
    // Ensure fields are serialized first before the child class properties
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Include)]
    public string id;

    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Include)]
    public string type;

    [DefaultValue(false)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool isPermanent;

    [DefaultValue(0.0f)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float duration;

    [DefaultValue(0.0f)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float interval;

    [DefaultValue(BuffType.None)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public BuffType buffType;

    [DefaultValue(null)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string buffIcon;

    [DefaultValue(null)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<SkillEvent, ActionConfig[]> events;

    [DefaultValue(null)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<ModifierPropertyType, float> properties;

    [DefaultValue(null)]
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<ModifierStateType, ModifierStateValue> states;

    [JsonIgnore]
    public Sprite buffIconSprite;

    public virtual ModifierConfig Clone()
    {
        ModifierConfig clone = (ModifierConfig)this.MemberwiseClone();
        if (this.properties != null)
        {
            clone.properties = new Dictionary<ModifierPropertyType, float>(this.properties);
        }

        if (this.states != null)
        {
            clone.states = new Dictionary<ModifierStateType, ModifierStateValue>(this.states);
        }

        return clone;
    }
}