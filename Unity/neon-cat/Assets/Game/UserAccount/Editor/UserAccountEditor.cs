using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserAccounts))]
public class UserAccountEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Factory reset"))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs erased");
        }

        if (GUILayout.Button("Print current account JSON"))
        {
            Debug.Log($"{JsonUtility.ToJson(UserAccounts.Instance.GetCurrentAccount().AccountData, true)}");
        }
    }


}
