using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomEditor(typeof(HomeIslandControllerDev))]
    public class HomeIslandControllerDevEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var homeIslandControllerDev = target as HomeIslandControllerDev;
            if (GUILayout.Button("Print info"))
            {
                homeIslandControllerDev.PrintInfo();
            }
        }
    }


}

