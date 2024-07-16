using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class LevelsCsvImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (IsCsvLevels(assetPath))
                {
                    var home = GameObject.Find("Home");
                    if (home == null)
                    {
                        Debug.LogWarning("Can't reimport level order because unable to find Home object. Using previous order.");
                        return;
                    }
                    var levelManager = home.GetComponent<LevelManager>();
                    var csvTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    Assert.IsNotNull(csvTextAsset);
                    Import(levelManager, csvTextAsset);
                    Assert.IsNotNull(levelManager);
                    Debug.Log($"{assetPath} levels csv imported");
                }
            }
        }

        private static bool IsCsvLevels(string path)
        {
            if (!path.StartsWith("Assets/Game/Level/"))
                return false;
            if (!path.EndsWith(".csv"))
                return false;
            return true;
        }

        private static void Import(LevelManager levelManager, TextAsset csvTextAsset)
        {
            // if (levelManager == null || levelManager.ImportWriteProtection)
            // {
            //     Debug.LogWarning($"Can't find level manager or it has write protection to importing from CSV");
            //     return;
            // }
            //
            // var table = new SpreadSheetTable<CsvRowLevels>(new StringReader(csvTextAsset.text));
            // levelManager.LevelOrder = new List<LevelManager.LevelEntry>();
            // HashSet<string> levelNames = new HashSet<string>(table.Table().GetLineCount());
            //
            // var lineCounter = 0;
            // foreach (var dataLine in table.Table().Rows())
            // {
            //     lineCounter++;
            //     var levelName = dataLine.Level;
            //     if(levelName.Contains(levelName))
            //         Debug.LogError($"duplicate level name {levelName}. Fix it in the spreadsheet {csvTextAsset.name} and reimport");
            //
            //     if (string.IsNullOrEmpty(levelName))
            //     {
            //         Debug.LogWarning($"{lineCounter}: no level name");
            //         continue;
            //     }
            //
            //     var levelPrefab =
            //         AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Game/Level/Prefabs/{levelName}.prefab");
            //     if (levelPrefab == null)
            //     {
            //         Debug.LogError($"Couldn't load level prefab {levelName}");
            //         continue;
            //     }
            //     else
            //     {
            //         ValidateCoins(levelPrefab, dataLine.Coins);
            //         ValidateHostages(levelPrefab, dataLine.Hostages);
            //         levelManager.LevelOrder.Add(new LevelManager.LevelEntry
            //         {
            //             LevelPrefab = levelPrefab
            //         });
            //     }
            // }

            EditorUtility.SetDirty(levelManager);
        }

        private static void ValidateCoins(GameObject levelPrefab, int coinsInSpreadsheet)
        {
            var pickables = levelPrefab.GetComponentsInChildren<Pickable>();
            var coinsPresented = pickables.Count(x => x.NodeType == NodeType.Coin);
            if (coinsPresented != coinsInSpreadsheet)
                Debug.LogError(
                    $"There are {coinsPresented} coins in the level '{levelPrefab.name}', but regarding to the balance spreadsheet it should be {coinsInSpreadsheet}");
        }

        private static void ValidateHostages(GameObject levelPrefab, int hostagesInSpreadsheet)
        {
            var hostages = levelPrefab.GetComponentsInChildren<HostageController>();
            var hostagesPresented = hostages.Length;
            if (hostagesPresented != hostagesInSpreadsheet)
                Debug.LogError(
                    $"There are {hostagesPresented} hostages in the level '{levelPrefab.name}', but regarding to the balance spreadsheet it should be {hostagesInSpreadsheet}");
        }
    }
}