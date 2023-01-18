using UnityEditor;

[CustomPropertyDrawer(typeof(AttributeTypeToFloatDictionary))]
[CustomPropertyDrawer(typeof(SkillIdToSkillConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
