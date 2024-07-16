using System.Collections;
using Events;
using Game;
using Game.Level;
using Game.Level.Entities;
using GameLib;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class StateThumbnailGenerator : AppStateManager.AppState<StateThumbnailGenerator>, IHandle<PortalOut.EventSpawnPlayer>
{
    public LevelManager LevelManager;
    public RangeInt LevelsToGenerate;
    public ScreenshotController ScreenShotController;
    private bool _receivedSpawnPlayerEvent;
    private Level _currentLevel;


    public override void AppStateEnter()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
        StartCoroutine(IterateLevels());
    }

    public override void AppStateLeave()
    {
    }

    private IEnumerator IterateLevels()
    {
        for (int i = 0; i < LevelManager.LevelOrder.Count; i++)
        {
            _receivedSpawnPlayerEvent = false;

            var levelPrefab = LevelManager.GetLevel(i);
            if (levelPrefab.GetComponent<LevelGenerator>() != null)
            {
                Debug.Log($"Skipping thumbnail generation for survival level {levelPrefab.name}");
                continue;
            }

            StateGameplay.Instance.LevelID = i;
            StateGameplay.Instance.LevelPrefab = levelPrefab;
            StateGameplay.Instance.EntryCoins = 0;
            yield return null;



            Debug.Log($"Screenshoting level {levelPrefab.name}");
            AppStateManager.Instance.Start(StateGameplay.Instance);

            yield return new WaitUntil(() => (_receivedSpawnPlayerEvent ));

            yield return StartCoroutine(PreparingThumbnail());
        }
    }

    private IEnumerator PreparingThumbnail()
    {
        // get thumbnail camera
        var cameraThumbnail = _currentLevel.transform.Find("ThumbnailCamera")?.GetComponent<Camera>();
        if (cameraThumbnail == null)
        {
            Debug.LogError($"Can't find ThumbnailCamera in level '{_currentLevel.name}'");
            yield break;
        }

        // save texture level folder
        var saver = ScreenShotController.ScreenshotProcessors[0] as ScreenshotProcessorFilesCreator;
        saver.BaseFileName = $"Assets/Game/Level/Prefabs/{_currentLevel.name}";
        ScreenShotController.Screenshoter.OriginalCamera = cameraThumbnail;
        ScreenShotController.TakeScreenShot();
        yield return null; // note: sometimes we have not enough time to save texture, skip frame

        // change texture type
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath($"Assets/Game/Level/Prefabs/{_currentLevel.name}.png");
        importer.textureType = TextureImporterType.Sprite;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        // set to level settings
        var levelSettings = AssetDatabase.LoadAssetAtPath<LevelEnvironmentSettings>($"Assets/Game/Level/Prefabs/{_currentLevel.name}.prefab");
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Game/Level/Prefabs/{_currentLevel.name}.png");
        levelSettings.Thumbnail = sprite;
        var prefab = PrefabUtility.SavePrefabAsset(levelSettings.gameObject);
        EditorUtility.SetDirty(prefab);

    }

    public void Handle(PortalOut.EventSpawnPlayer message)
    {
        _receivedSpawnPlayerEvent = true;
        
        // get level which player is spawned in
        _currentLevel = message.SpawnedPlayer.GetComponentInParent<Level>();
        if (_currentLevel == null)
        {
            Debug.LogError("Can't find level of player");
            return;
        }
    }
}
#else

public class StateThumbnailGenerator : AppStateManager.AppState<StateThumbnailGenerator>
{
    public override void AppStateEnter() {}
    public override void AppStateLeave() {}
}
#endif