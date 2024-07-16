using GameLib.Alg;
using UnityEngine;
using UnityEngine.UI;

public class PreventMultipleClick : MonoBehaviour
{
    public enum PreventMethod
    {
        JustKeepStatus, // button is still totally functional but IsInteractable flag is changed
        ButtonInteractable, // set button interactble along with the IsInteractable flag ( sometimes  you don't want it if it affects the view of the button)
        ButtonEnable // Enalbe/ disable button script along with the IsInteractable flag

    }

    private Button _btn;
    public PreventMethod Prevent;

    public bool IsInteractable { get; private set; }


    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        IsInteractable = _btn.interactable;
        EnableButtonActivity(IsInteractable);
    }

    void OnEnable()
    {
        Recharge();
    }

    public void Recharge()
    {
        EnableButtonActivity(true);
    }

    void OnClick()
    {
        Debug.Log($"Prevent multiple click for {gameObject.transform.GetDebugName()}");
        EnableButtonActivity(false);
    }
 
    private void EnableButtonActivity(bool flag)
    {
        IsInteractable = flag;
        if (Prevent == PreventMethod.JustKeepStatus)
            return;
        else if (Prevent == PreventMethod.ButtonInteractable)
            _btn.interactable = flag;
        else if (Prevent == PreventMethod.ButtonEnable)
            _btn.enabled = flag;
    }
}
