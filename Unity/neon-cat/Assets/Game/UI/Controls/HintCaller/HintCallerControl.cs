using DG.Tweening;
using GameGUI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HintCallerControl : MonoBehaviour
{
    public RectTransform Hint;
    public SimpleGUI OverlayGUI;
    private Button _button;
    private Tween _scaleDown;

    void Awake()
    {
        _button = GetComponent<Button>();
        Assert.IsNotNull(_button);
        _button.onClick.AddListener(OnButtonPress);
    }

    void OnButtonPress()
    {
        Debug.Log("Hint tap");

        // process text in the hint
        var text = Hint.GetComponentInChildren<TextMeshProUGUI>();
        text.text = string.Format(text.text, UserAccounts.Instance.GetCurrentAccount().AccountData.IslandLevel);

        Hint.gameObject.SetActive(true);
        Hint.transform.localScale = Vector3.one;
        Hint.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).From().SetUpdate(true);
        OverlayGUI.GetCurrentScreen().IsInputEnabled = false;
        _button.interactable = false;
    }

    public void OnBubbleTap()
    {
        if (_scaleDown != null && _scaleDown.IsPlaying())
            return;
        Debug.Log("Bubble tap");
        _button.interactable = true;
        _scaleDown = Hint.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() => OverlayGUI.GetCurrentScreen().IsInputEnabled = true);
    }
}
