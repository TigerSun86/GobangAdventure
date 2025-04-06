#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
        {
            var oldColor = GUI.color;
            GUI.color = Color.red;
            EditorGUI.PropertyField(position, property, label);
            GUI.color = oldColor;

            // Optional: show warning in console
            Debug.LogWarning($"{property.name} is required!", property.serializedObject.targetObject);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }

        EditorGUI.EndProperty();
    }
}
#endif
