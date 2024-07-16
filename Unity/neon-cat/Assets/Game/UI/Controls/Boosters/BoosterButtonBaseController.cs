using Events;
using Game;
using GameGUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoosterButtonBaseController : MonoBehaviour, IHandle<InputHandler.EventInputBoosterPress>, SimpleGUI.IInitialize
{
    public BoosterManager.Booster Booster;
    public PreventMultipleClickCooldown PreventMultiClick;
    public BoosterButtonView View;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    void Update()
    {
        var price = BoosterManager.Instance.GetBoosterUseCost(Booster);
        var isEnoughCoins = StateGameplay.Instance.IsEnoughCoins(price);
        var isTimeNormal = StateGameplay.Instance.Player.GetState() == PlayerController.State.TimeNormal;
        View.Button.interactable = PreventMultiClick.IsInteractable && isEnoughCoins && isTimeNormal;
    }

    public void OnBoosterTap()
    {
        var price = BoosterManager.Instance.GetBoosterUseCost(Booster);
        var isEnoughCoins = StateGameplay.Instance.IsEnoughCoins(price);
        if (isEnoughCoins)
        {
            StateGameplay.Instance.SpendCoins(price);
            BoosterManager.Instance.ActivateBooster(Booster);
        }
    }

    public void Handle(InputHandler.EventInputBoosterPress message)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(View.Button.gameObject, pointer, ExecuteEvents.submitHandler);
    }
}
