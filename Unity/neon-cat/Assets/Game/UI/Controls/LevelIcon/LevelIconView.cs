using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelIconView : MonoBehaviour
{
    public TextMeshProUGUI MainText;
    public Image SpiralBackground;
    public Image LastNotCompletedBackground;
    public Image Medal0;
    public StarsBlockView StarsBlockView;
    public Color SpiralColor;
    public Animator Animator;


    void OnValidate()
    {
        SpiralBackground.color = SpiralColor;
    }


    public void SetMainText(string text, bool isTextVisible)
    {
        MainText.text = text;
        MainText.gameObject.SetActive(isTextVisible);
    }

    public void SetMainTextVisible(bool flag)
    {
        MainText.gameObject.SetActive(flag);
    }

    public void ShowSpiral(bool flag)
    {
        SpiralBackground.color = SpiralColor;
        SpiralBackground.gameObject.SetActive(flag);
    }

    public void ShowMedal0(bool flag)
    {
        Medal0.gameObject.SetActive(flag);
    }

    public void ShowLastNotComleted(bool flag)
    {
        LastNotCompletedBackground.gameObject.SetActive(flag);
    }

    public void SetStarsVisible(bool flag)
    {
        StarsBlockView.gameObject.SetActive(flag);
    }

    public void PlayAnimation()
    {
        Animator.SetTrigger("StartPlay");
    }
}
