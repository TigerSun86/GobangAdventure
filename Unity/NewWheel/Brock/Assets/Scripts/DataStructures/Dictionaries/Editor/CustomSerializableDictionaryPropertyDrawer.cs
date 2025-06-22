using UnityEditor;

[CustomPropertyDrawer(typeof(SkillTypeToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(StringToSkillConfigDictionary))]
[CustomPropertyDrawer(typeof(StringToWeaponConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
