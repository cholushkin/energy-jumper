// using Assets.Plugins.Alg;
// using IngameDebugConsole;
// using UnityEditor;
// using UnityEngine;
//
// public static class DbgContentRoutines
// {
//     [ConsoleMethod("content.meshes", "Checks current loaded scene for missing meshes on MeshFilters")]
//     public static void MissingMeshes()
//     {
// #if UNITY_EDITOR
//         var meshFilters = Object.FindObjectsOfType<MeshFilter>();
//         Debug.Log($"{meshFilters.Length} MeshFilters found");
//
//         var missingCounter = 0;
//         foreach (MeshFilter mf in meshFilters)
//         {
//             if (mf.sharedMesh == null)
//             {
//                 ++missingCounter;
//                 Debug.Log($"{mf.transform.GetDebugName()} has empty Mesh in MeshFilter.");
//             }
//         }
//         Debug.Log($"{missingCounter} missing meshes found");
// #else
//         Debug.Log("Supported in editor only!");
// #endif
//     }
// }