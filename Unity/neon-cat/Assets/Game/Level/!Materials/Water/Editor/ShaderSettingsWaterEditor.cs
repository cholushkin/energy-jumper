using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShaderSettingsWater))]
public class ShaderSettingsWaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var settingsScript = target as ShaderSettingsWater;

        if (GUILayout.Button("Set material back TO DEFAULT"))
        {
            Debug.Log("Setting default");
            settingsScript.SetMaterialToDefault();
        }

        if (GUILayout.Button("Set material TO VALUES"))
        {
            Debug.Log("Setting material to values from the script");
            settingsScript.SetToMaterial();
        }

        if (GUILayout.Button("Set settings FROM MATERIAL"))
        {
            Debug.Log("Setting Settings from material configuration");
            settingsScript.SetFromMaterial();
        }
    }
}

