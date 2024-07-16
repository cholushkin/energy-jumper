using System;
using System.Collections.Generic;
using Game;
using IngameDebugConsole;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public class LevelEntry
    {
        public GameObject LevelPrefab;
    }

    public const int LevelsPerChapter = 40;
    public const int ChapterCount = 3;
    public const int SurvivalLevelCount = 3;
    public int[] SurvivalUnlockIndexes = { 10, 50, 130 };

    public List<LevelEntry> LevelOrder;
    public List<LevelEntry> SurvivalLevelOrder;
    [Tooltip("Set it to true if you don't want CSV level order rewrite your custom (probably for debug purposes) level order on reimport")]
    public bool ImportWriteProtection;


    private void Awake()
    {
        //DebugLogConsole.AddCommand("levels.list", "Prints a list of registered levels", PrintListOflevels);
    }

    public GameObject GetLevel(int levelIndex)
    {
        return LevelOrder[levelIndex].LevelPrefab;
    }

    public GameObject GetSurvivalLevel(StateGameplay.GameMode mode)
    {
        if (mode == StateGameplay.GameMode.SurvivalA)
            return SurvivalLevelOrder[0].LevelPrefab;
        if (mode == StateGameplay.GameMode.SurvivalB)
            return SurvivalLevelOrder[1].LevelPrefab;
        if (mode == StateGameplay.GameMode.SurvivalC)
            return SurvivalLevelOrder[2].LevelPrefab;
        return null;
    }

    public int GetMaxLevelIndex()
    {
        return LevelOrder.Count - 1;
    }

    public int GetMaxSurvivalLevelIndex()
    {
        return SurvivalLevelOrder.Count - 1;
    }

    public void PrintListOflevels()
    {
        int index = 0;
        foreach (var level in LevelOrder)
            Debug.Log($"{index++}. {level.LevelPrefab.name}");
        Debug.Log($"##### Levels count: {LevelOrder.Count}");

        index = 0;
        foreach (var survivalLevel in SurvivalLevelOrder)
            Debug.Log($"{index++}. {survivalLevel.LevelPrefab.name}");
        Debug.Log($"##### Survival levels count: {SurvivalLevelOrder.Count}");
    }
}
