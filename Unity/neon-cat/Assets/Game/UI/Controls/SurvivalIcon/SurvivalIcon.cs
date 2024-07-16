using Game;
using TMPro;
using UnityEngine;

public class SurvivalIcon : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void SetMode(StateGameplay.GameMode mode)
    {
        // todo: different icon
    }

    public void SetLevelNumber( int levelIndexNumber)
    {
        Text.text = (levelIndexNumber + 1).ToString();
    }
}
