using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameEvent obj = (GameEvent)target;

        if (GUILayout.Button("Raise()"))
        {
            obj.Raise();
        }
    }
}