using UnityEditor;

[CustomPropertyDrawer(typeof(AttributeTypeToFloatDictionary))]
[CustomPropertyDrawer(typeof(SkillIdToAttributesDictionary))]
[CustomPropertyDrawer(typeof(SkillIdToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(SkillIdToSkillConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
