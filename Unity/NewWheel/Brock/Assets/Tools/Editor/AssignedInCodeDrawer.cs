#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom drawer that adds an icon to the property label.
/// </summary>
[CustomPropertyDrawer(typeof(AssignedInCodeAttribute))]
public class AssignedInCodeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var iconAttr = (AssignedInCodeAttribute)attribute;

        // Prefix the label with the icon
        label.text = $"{iconAttr.icon} {label.text}";

        // Required for prefab override handling
        EditorGUI.BeginProperty(position, label, property);

        // Draw the property (Unity handles foldout)
        EditorGUI.PropertyField(position, property, label, includeChildren: true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get height including children if expanded
        return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
    }
}
#endif
