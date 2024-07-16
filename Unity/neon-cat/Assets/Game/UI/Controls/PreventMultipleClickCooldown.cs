using GameLib.Alg;
using UnityEngine;
using UnityEngine.UI;

public class PreventMultipleClickCooldown : MonoBehaviour
{
    public bool IsInteractable { get; private set; }
    public bool JustKeepStatus; // just keep status but don't change real button interactable state
    public float Cooldown;
    private Button _btn;
    
    private float _cooldownVal;
    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _cooldownVal = Cooldown;
        IsInteractable = _btn.interactable;
    }

    void OnEnable()
    {
        Recharge();
    }

    public void Recharge()
    {
        if(!JustKeepStatus)
            _btn.interactable = true;
        IsInteractable = true;
    }
    void OnClick()
    {
        Debug.Log($"Prevent multiple click for {gameObject.transform.GetDebugName()}");
        if (!JustKeepStatus)
            _btn.interactable = false;
        IsInteractable = false;
        Cooldown = _cooldownVal;
    }

    void Update()
    {
        if(Cooldown < 0f)
            return;
        Cooldown -= Time.deltaTime;
        if (Cooldown < 0)
        {
            Recharge();
        }
    }
}
