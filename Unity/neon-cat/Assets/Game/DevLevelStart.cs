using Game;
using Game.Level;
using GameLib;
using UnityEngine;

[ScriptExecutionOrder(-100)]
public class DevLevelStart : MonoBehaviour
{
    public StateGameplay Gameplay;
    public bool IsDevStart { get; private set; }
    void Awake()
    {
        // auto DevStart (run level from the scene instead of prefab)
        // find all levels and disable them before they call awake (because some objects in level could start spawn events)
        // that's why we need execution order before Level (Level depends on DevLevelStart)
        if (GetComponentInParent<AppStateManager>().StartState == transform) // if starting state is StateGameplay then run the level from the scene
        {
            IsDevStart = true;
            var levels = FindObjectsOfType<Level>(true);
            foreach (var level in levels)
            {
                // assign start level to first active (if user didn't assign it)
                if (Gameplay.LevelPrefab == null && level.gameObject.activeSelf)
                {
                    Gameplay.LevelPrefab = level.gameObject;
                }
                level.gameObject.SetActive(false);
            }

            if (Gameplay.LevelPrefab == null && levels.Length > 0) // all levels are disabled
                Gameplay.LevelPrefab = levels[0].gameObject; // just take first one

            if (Gameplay.LevelPrefab == null)
                Debug.LogError($"There is no level to DevStart");
            else
                Debug.Log($"Dev Start on level {Gameplay.LevelPrefab.name}");
            
            Gameplay.LevelID = -1;
        }
    }
}
