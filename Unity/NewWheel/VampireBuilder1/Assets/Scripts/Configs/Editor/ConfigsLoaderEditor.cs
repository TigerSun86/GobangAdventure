using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConfigsLoader))]
public class ConfigsLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ConfigsLoader obj = (ConfigsLoader)target;
        if (GUILayout.Button("Load"))
        {
            obj.Load();
        }
    }
}