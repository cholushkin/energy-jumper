using DG.Tweening;
using TMPro;
using UnityEngine;

public class IconPopupMessage : MonoBehaviour
{
    public float Duration;
    public TextMeshProUGUI Text;
    public Ease ColoringEase;

    void Reset()
    {
        Duration = 1f;
        ColoringEase = Ease.InOutCubic;
        Text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        var t = Text.DOColor(new Color(1, 1, 1, 0), Duration).SetEase(ColoringEase);
        t.SetUpdate(true);
        t.OnComplete(() => Destroy(gameObject));
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
