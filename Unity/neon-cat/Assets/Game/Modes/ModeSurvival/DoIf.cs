using System;
using UnityEngine;

public class DoIf : MonoBehaviour
{
    public enum Condition
    {
        None,
        ChunkIsMirrored,
        ChunkIsNotFlipped,
        ChunkIsRotated,
        ChunkIsNotRotated,
        ChunkIsShrinked,
        ChunkIsNotShrinked,
        CurrentIsActive,
        CurrentIsNotActive,
        TargetIsActive,
        TargetIsNotActive,
    }

    [Flags]
    public enum Action
    {
        None,
        DisableTarget = 1,
        DisableCurrent = 2,
        EnableTarget = 4,
        EnableCurrent = 8,
        DestroyTarget = 16,
        DestroyCurrent = 32
    }

    public Condition[] IF; // if cond1 and cond2 and so on...
    public Action DO;
    public GameObject Target; // Action target
    public ChunkRandomization ChunkRandomization; // Chunk radomization controller (for conditions starting from Chunk*)


    void Reset()
    {
        Target = gameObject;
        ChunkRandomization = GetComponentInParent<ChunkRandomization>();
    }

    public bool DoConditionally()
    {
        bool conditionPassed = true;


        foreach (var condition in IF)
        {
            switch (condition)
            {
                case Condition.ChunkIsMirrored:
                    conditionPassed &= IsChunkFlipped();
                    break;
                case Condition.ChunkIsNotFlipped:
                    conditionPassed &= !IsChunkFlipped();
                    break;
                case Condition.ChunkIsRotated:
                    conditionPassed &= IsChunkRotated();
                    break;
                case Condition.ChunkIsNotRotated:
                    conditionPassed &= !IsChunkRotated();
                    break;
                case Condition.ChunkIsShrinked:
                    conditionPassed &= IsChunkShrinked();
                    break;
                case Condition.ChunkIsNotShrinked:
                    conditionPassed &= !IsChunkShrinked();
                    break;
                case Condition.CurrentIsActive:
                    conditionPassed &= gameObject.activeInHierarchy;
                    break;
                case Condition.CurrentIsNotActive:
                    conditionPassed &= !gameObject.activeInHierarchy;
                    break;
                case Condition.TargetIsActive:
                    conditionPassed &= Target.activeInHierarchy;
                    break;
                case Condition.TargetIsNotActive:
                    conditionPassed &= !Target.activeInHierarchy;
                    break;
                default:
                    Debug.LogError($"{condition} is not implemented");
                    break;
            }

            if (!conditionPassed) // lazy conditions processing
                return false;
        }

        Do();
        return true;
    }

    public void Do()
    {
        if (DO.HasFlag(Action.DisableTarget))
            Target.SetActive(false);
        if (DO.HasFlag(Action.DisableCurrent))
            gameObject.SetActive(false);
        if (DO.HasFlag(Action.EnableTarget))
            Target.SetActive(true);
        if (DO.HasFlag(Action.EnableCurrent))
            gameObject.SetActive(true);
        if (DO.HasFlag(Action.DestroyTarget))
            Destroy(Target);
        if (DO.HasFlag(Action.DestroyCurrent))
            Destroy(gameObject);
    }

    #region ######################## Condition checks
    private bool IsChunkRotated()
    {
        return ChunkRandomization.IsRotated;
    }

    private bool IsChunkShrinked()
    {
        return ChunkRandomization.IsShrinked;
    }

    private bool IsCurrentEnabled()
    {
        return gameObject.activeInHierarchy;
    }

    private bool IsChunkFlipped()
    {
        return ChunkRandomization.transform.localScale.x < 0;
    }



    #endregion
}
