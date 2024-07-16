using GameLib.Alg;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public Material HologramMaterial;

    public void ApplyHologramRevealEffect(GameObject gameObj, bool recursively)
    {
        void AddCompIfNotAdded(GameObject gObj)
        {
            var comp = gObj.AddSingleComponentSafe<HologramRevealEffect>(out var wasAdded);
            if (wasAdded)
                comp.Init(HologramMaterial);
        }
        if (recursively)
            gameObj.transform.ForEachChildrenRecursive(t => AddCompIfNotAdded(t.gameObject));
        else
            AddCompIfNotAdded(gameObj);
    }

    public void ApplyHologramHideEffect(GameObject gameObj, bool recursively)
    {
        void AddCompIfNotAdded(GameObject gObj)
        {
            var comp = gObj.AddSingleComponentSafe<HologramHideEffect>(out var wasAdded);
            if (wasAdded)
                comp.Init(HologramMaterial);
        }
        if (recursively)
            gameObj.transform.ForEachChildrenRecursive(t => AddCompIfNotAdded(t.gameObject));
        else
            AddCompIfNotAdded(gameObj);
    }
}
