using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Print info"))
        {
            var levMan = target as LevelManager;
            levMan?.PrintListOflevels();
        }
    }
}
