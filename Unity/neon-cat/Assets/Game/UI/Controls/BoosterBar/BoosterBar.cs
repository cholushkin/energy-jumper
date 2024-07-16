using UnityEngine;

namespace GameGUI
{
    public class BoosterBar : MonoBehaviour
    {
        public void AddBoosterIndicator(IconIndicatorBoosterView boosterIndicator)
        {
            Debug.Log("adding booster indicator");
        }

        public void RemoveBoosterIndicator(IconIndicatorBoosterView boosterIndicator)
        {
            Debug.Log("removing booster indicator");
        }
    }
}