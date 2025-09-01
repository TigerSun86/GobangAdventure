using UnityEditor;

[CustomPropertyDrawer(typeof(BuffTypeToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(BuffTypeToSpriteDictionary))]
[CustomPropertyDrawer(typeof(SkillTypeToGameObjectDictionary))]
[CustomPropertyDrawer(typeof(StringToSkillConfigDictionary))]
[CustomPropertyDrawer(typeof(StringToEnemyConfigDictionary))]
[CustomPropertyDrawer(typeof(StringToItemConfigDictionary))]
[CustomPropertyDrawer(typeof(StringToWeaponConfigDictionary))]
public class CustomSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{ }
