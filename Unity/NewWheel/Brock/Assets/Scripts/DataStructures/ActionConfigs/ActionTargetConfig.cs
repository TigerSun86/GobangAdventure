using System;
using Newtonsoft.Json;

[Serializable]
public class ActionTargetConfig
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public ActionTargetType actionTargetType;
}
