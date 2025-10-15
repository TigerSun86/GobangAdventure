using System;
using Newtonsoft.Json;

[Serializable]
public class ActionConfig
{
    // Ensure 'type' and 'actionTargetConfig' are serialized first before the child class properties
    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Include)]
    public string type;

    [JsonProperty(Order = -2, DefaultValueHandling = DefaultValueHandling.Include)]
    public ActionTargetConfig actionTargetConfig;
}
