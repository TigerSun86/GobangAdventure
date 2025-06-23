using UnityEditor;

[CustomPropertyDrawer(typeof(BuffTypeToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(SkillTypeToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(StringToSkillConfigDictionary))]
[CustomPropertyDrawer(typeof(StringToWeaponConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
