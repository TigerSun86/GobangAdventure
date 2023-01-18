using UnityEditor;

[CustomPropertyDrawer(typeof(AttributeTypeFloatDictionary))]
[CustomPropertyDrawer(typeof(StringSkillConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
