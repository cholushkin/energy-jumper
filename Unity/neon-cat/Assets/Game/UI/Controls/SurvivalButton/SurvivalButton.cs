using Game;
using TMPro;
using UnityEngine;

public class SurvivalButton : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public StateGameplay.GameMode Mode;
    public void SetLevelNumber(int lastLevelIndex)
    {
        Text.text = (lastLevelIndex + 1).ToString();
    }
}
