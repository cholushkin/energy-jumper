using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelDistanceProgressionBarController : MonoBehaviour
{
    public enum State
    {
        Undefined,
        Initilizizing,
        Complete,
        Active
    }

    public LevelDistanceProgressionBarView View;
    private Transform _target; //static (never moves)
    private Transform _agent;
    private State _state;
    private float _maximumDistance;
    


    public void Awake()
    {
        // note: awake should be called after target and agent were being set
        Assert.IsTrue(_state == State.Initilizizing);
        Assert.IsNotNull(_target);
        Assert.IsNotNull(_agent);

        Assert.IsNotNull(View);
        View.SetInitialPosition();
    }

    public void OnDisable()
    {
        _target = _agent = null;
        _state = State.Undefined;
        View.SetProgression(0f);
    }


    public void SetTarget(Transform beaconTransform)
    {
        _state = State.Initilizizing;
        _target = beaconTransform;
    }

    public void SetAgent(Transform agentTransform)
    {
        _state = State.Initilizizing;
        _agent = agentTransform;
    }

    public void SetComplete()
    {
        _state = State.Complete;
        View.SetProgression(1f);
    }

    public void Update()
    {
        if (_target == null)
            return;
        if (_agent == null)
            return;

        // update state
        if (_state == State.Initilizizing)
        {
            _maximumDistance = CalculateCurrentDistance(); // first time call is a maximum (initial distance)
            _state = State.Active;
        }
        else if (_state == State.Active)
        {
            var distance = Mathf.Min(CalculateCurrentDistance(), _maximumDistance);
            var progression = 1f - Mathf.Clamp01(distance / _maximumDistance);
            View.SetProgression( progression );
            MusicController.Instance.SetCloserToDestination(progression); // todo: move it to player controller
        }
        else if (_state == State.Complete)
        {
            View.SetProgression(1f);
        }
    }

    private float CalculateCurrentDistance()
    {
        return (_target.position - _agent.position).magnitude;
    }

    public void SetLevelsToCurrent()
    {
        View.SetLevelsTexts(StateGameplay.Instance.Session.LevelID, StateGameplay.Instance.Session.LevelID + 1);
    }
}
