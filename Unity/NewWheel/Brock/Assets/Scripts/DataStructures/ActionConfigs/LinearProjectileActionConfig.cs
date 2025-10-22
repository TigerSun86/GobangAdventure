using Newtonsoft.Json;
using UnityEngine;

public class LinearProjectileActionConfig : ActionConfig
{
    public float moveSpeed;

    public string spritePath;

    [JsonIgnore]
    public Sprite sprite;
}