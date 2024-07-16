using System;
using System.Security.Cryptography;
using Game;
using UnityEngine;

public class UserAccount : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        [Serializable]
        public class LevelState
        {
            public LevelState(int chapter, int level)
            {
                ChapterId = chapter;
                LevelId = level;
                IsOpened = false;
                IsCompleted = false;
                DieCounter = 0;
                RestartCounter = 0;
                WinCounter = 0;
                Stars = 0;
                Medal0 = false;
            }

            public int ChapterId;
            public int LevelId;
            public bool IsOpened;
            public bool IsCompleted;
            public int DieCounter;
            public int RestartCounter; // enter to the level from the menu or restart from menu
            public int WinCounter; // enter to the level from the menu or restart from menu
            public int Stars;
            public bool Medal0; // medal for hostages
        }

        [Serializable]
        public class SurvivalLevelState
        {
            public SurvivalLevelState()
            {
                LastLevelIndex = -1;
            }
            public int LastLevelIndex;
            public int PlayCounter;
        }

        [Serializable]
        public class OptionsState
        {
            public bool Sound;
            public bool Music;
            public int Graphics;
        }


        public int AccountID; // slot number
        public string PlayerName;
        public int IslandLevel;
        public int IslandUpgradeIndex;
        public int Coins;
        public int LastChapterPageOpened;
        public LevelState[] LevelStates;
        public SurvivalLevelState[] SurvivalLevelStates;
        public int LastLevelIndex;
        public OptionsState Options;
        public int CheatedCounter;
    }

    public Data AccountData;
    public readonly int DataFormatVersion = 1;

    public void Save()
    {
        Debug.Log($"Saving data DefaultSaveData{ AccountData.AccountID}");
        var dataString = JsonUtility.ToJson(AccountData);
        PlayerPrefs.SetString($"DefaultSaveData{AccountData.AccountID}", dataString);
        PlayerPrefs.SetInt($"DefaultSaveDataFormatVersion{AccountData.AccountID}", DataFormatVersion);
    }

    public void Load()
    {
        var saveData = PlayerPrefs.GetString($"DefaultSaveData{AccountData.AccountID}");

        if (string.IsNullOrEmpty(saveData))
        {
            Debug.Log($"No save data found for user {AccountData.AccountID}. Creating new.");
            AccountData = CreateInitialData();
        }
        else
        {
            var saveDataFormatVersion = PlayerPrefs.GetInt($"DefaultSaveDataFormatVersion{AccountData.AccountID}", 0);
            if (saveDataFormatVersion < DataFormatVersion)
            {
                Debug.LogWarning($"Stored SaveData for account {AccountData.AccountID} has version {saveDataFormatVersion} which is less than current game data format version {DataFormatVersion}. Trying incremental convertion");
                Debug.LogWarning("Creating new save for now");
                AccountData = CreateInitialData();
                Save();
                // todo: IncrementalDataConvert();
            }
            else if (saveDataFormatVersion > DataFormatVersion)
            {
                Debug.LogWarning($"Stored SaveData for account {AccountData.AccountID} has version {saveDataFormatVersion} which is higher than current game data format version {DataFormatVersion}. Update the game to play your old save");
                Debug.LogWarning("Creating new save for now");
                AccountData = CreateInitialData();
                Save();
                // todo: warning popup "update the game to play old saved game
            }
            else if (saveDataFormatVersion == DataFormatVersion)
            {
                AccountData = JsonUtility.FromJson<Data>(saveData);
            }
        }
    }

    public void LoadAndMerge()
    {
        // todo: data from cloud should be merged with local data and resaved
    }

    private Data CreateInitialData()
    {
        // lev states
        Data.LevelState[] levStates = new Data.LevelState[LevelManager.LevelsPerChapter * LevelManager.ChapterCount];
        int chapter = 0, level = 0;
        for (int i = 0; i < levStates.Length; ++i)
        {
            levStates[i] = new Data.LevelState(chapter, level++);
            if (level >= LevelManager.LevelsPerChapter)
            {
                level = 0;
                chapter++;
            }
        }
        levStates[0].IsOpened = true;

        // survival level states
        var survivalLevelStates = new Data.SurvivalLevelState[LevelManager.SurvivalLevelCount];
        for (int i = 0; i < survivalLevelStates.Length; ++i)
            survivalLevelStates[i] = new Data.SurvivalLevelState();

        // options
        var options = new Data.OptionsState();
        options.Sound = true;
        options.Music = true;
        options.Graphics = 2;

        return new Data
        {
            PlayerName = "DefaultPlayer",
            AccountID = 0,
            IslandLevel = 0,
            IslandUpgradeIndex = 1,
            Coins = 0,
            LevelStates = levStates,
            SurvivalLevelStates = survivalLevelStates,
            Options = options
        };
    }
}


public static class UserDataHelper
{
    public static UserAccount.Data.SurvivalLevelState GetSurvivalLevelState(this UserAccount.Data data, StateGameplay.GameMode mode)
    {
        switch (mode)
        {
            case StateGameplay.GameMode.SurvivalA:
                return data.SurvivalLevelStates[0];
            case StateGameplay.GameMode.SurvivalB:
                return data.SurvivalLevelStates[1];
            case StateGameplay.GameMode.SurvivalC:
                return data.SurvivalLevelStates[2];
        }

        return null;
    }
}