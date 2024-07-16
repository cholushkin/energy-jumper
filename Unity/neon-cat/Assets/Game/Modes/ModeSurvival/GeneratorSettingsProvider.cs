using System;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;

public class GeneratorSettingsProvider : MonoBehaviour
{
    [Serializable]
    public class HeightSettingsTable
    {
        public int StartingFromLevel;
        public List<GameObject> Prefabs;
    }

    [Serializable]
    public class LevelSettingsTable // settings based on level index
    {
        public int StartingFromLevel;
        public int MinChunks; // when generator reach this value new cycle is not starting
    }

    public List<HeightSettingsTable> HeightSettings;
    public List<LevelSettingsTable> LevelSettings;

    public GameObject GetRndPrefab(int level, IPseudoRandomNumberGenerator rnd)
    {
        for (int i = HeightSettings.Count - 1; i >= 0; --i)
            if (HeightSettings[i].StartingFromLevel <= level)
                return rnd.FromList(HeightSettings[i].Prefabs);
        return null;
    }

    public LevelSettingsTable GetLevelSettingsTable(int levelIndex)
    {
        for (int i = LevelSettings.Count - 1; i >= 0; --i)
            if (LevelSettings[i].StartingFromLevel <= levelIndex)
                return LevelSettings[i];
        return null;
    }
}
