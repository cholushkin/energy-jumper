using UnityEngine;


namespace Game
{
    // MainGameObject is a logical owner of colliders (usually with RigidBody)
    // Example:
    // - Player (RigidBody)
    //    - Magnet (Collider) // mainGameObject == Magnet
    //    - PlayerVisual (Collider) // mainGameObject == Player
    //    - Antigrav (Collider) // mainGameObject == Antigrav
    public class MainGameObjectPropagator : MonoBehaviour
    {
        public GameObject MainGameObject;
    }

    public static class MainGameObjectPropagatorHelper
    {
        public static GameObject GetMainGameObject(this GameObject gObj)
        {
            var gPropagator = gObj.GetComponent<MainGameObjectPropagator>();
            if (gPropagator == null)
                return gObj;
            return gPropagator.MainGameObject;
        }

    }
}