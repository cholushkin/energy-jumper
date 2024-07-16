using DG.Tweening;
using Events;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class HostageBubble : MonoBehaviour, IHandle<InputHandler.EventInputCloseBubble>
{
    public enum State
    {
        HiddenPending,
        Showing,
        Done
    }

    public int ShowNumber = 1;
    public Camera Camera;
    public Transform Target;
    public TextMeshProUGUI Text;
    public LocalizeStringEvent LocalizeStringEvent;
    private State _currentState;
    private StateMachineImmediate<State> _stateMachine;
    private float _bubbleLifeTime;
    private float MaxLifeTime = -1f; // -1 means infinite time


    void Init(Transform target, string localizationKey)
    {
        Target = target;
        Camera = GameCamera.Instance.Camera;
        _stateMachine = new StateMachineImmediate<State>(this, State.HiddenPending);
        gameObject.SetActive(false);
        GlobalEventAggregator.EventAggregator.Subscribe(this, notifyDisabled: false);
        LocalizeStringEvent.StringReference.SetReference("Hostages", localizationKey);
    }


    void Update()
    {
        _stateMachine.Update();
    }

    public static HostageBubble Create(Transform target, string localizationKey)
    {
        var hostageBubblePrefab = GamePrefabs.Instance.GameEntities["HostageBubble"];
        var hostageBubbleObj = Instantiate(hostageBubblePrefab, StateGameplay.Instance.ScreenBubbleDialog.transform);
        var hostageBubble = hostageBubbleObj.GetComponent<HostageBubble>();
        hostageBubble.Init(target, localizationKey);
        return hostageBubble;
    }

    public void Show()
    {
        if (_stateMachine.CurrentState.State == State.HiddenPending)
        {
            if (ShowNumber == 0)
            {
                _stateMachine.GoTo(State.Done);
                return;
            }

            _stateMachine.GoTo(State.Showing);
        }
    }

    public void Hide()
    {
        if (_stateMachine.CurrentState.State == State.Showing)
            _stateMachine.GoTo(State.HiddenPending);
    }

    public void SetWaitTime(float bubbleWaitTime)
    {
        MaxLifeTime = bubbleWaitTime;
        _bubbleLifeTime = 0f; // reset show time
    }

    public double GetWaitTime()
    {
        return MaxLifeTime;
    }

    #region StateMachine states

    void OnEnterHiddenPending()
    {
        PlayHideAnimation(() => gameObject.SetActive(false));
    }

    void OnEnterShowing()
    {
        PlayShowAnimation();
        _bubbleLifeTime = 0f; // reset show time
        ShowNumber--;
        _currentScreenPos = Camera.WorldToScreenPoint(Target.transform.position);
    }

    private Vector3 _currentScreenPos;
    private Vector3 _velocity;
    private float SmoothTime = 0.1f;

    void OnUpdateShowing()
    {
        if (Target == null)
            return;

        _bubbleLifeTime += Time.deltaTime;
        if (MaxLifeTime != -1f && _bubbleLifeTime >= MaxLifeTime)
        {
            _stateMachine.GoTo(State.Done);
            return;
        }

        var screenPos = Camera.WorldToScreenPoint(Target.transform.position);

        _currentScreenPos = Vector3.SmoothDamp(_currentScreenPos, screenPos,
            ref _velocity, SmoothTime, 99999f, Time.unscaledDeltaTime);


        transform.position = _currentScreenPos;
    }

    void OnEnterDone()
    {
        PlayHideAnimation(() => gameObject.SetActive(false));
    }

 
    #endregion


    void PlayHideAnimation(TweenCallback endCallback)
    {
        const float duration = 0.3f;
        transform.DOScale(Vector3.one * 0.65f, duration).OnComplete(endCallback).SetEase(Ease.InBack);
    }

    private void PlayShowAnimation()
    {
        const float duration = 0.6f;
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * 0.4f;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).SetUpdate(true);
        Text.DOFade(0f, duration *2f).From().SetUpdate(true);
    }

    public void OnPress()
    {
        StateGameplay.Instance.SetPause(false);
        StateGameplay.Instance.MainWindowSystem.PopScreen("Screen.BubbleDialog");
        StateGameplay.Instance.FocusOnPlayer();
        Hide();
    }

    public void Handle(InputHandler.EventInputCloseBubble message)
    {
        GlobalEventAggregator.EventAggregator.Unsubscribe(this);
        OnPress(); // simulate tap on the bubble
    }
}
