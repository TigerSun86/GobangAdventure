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

        // Draw the field normally
        EditorGUI.PropertyField(position, property, label);
    }
}
#endif
