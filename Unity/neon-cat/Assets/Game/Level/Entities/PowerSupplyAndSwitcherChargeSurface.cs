using Game;
using UnityEngine;
using UnityEngine.Assertions;


public class PowerSupplyAndSwitcherChargeSurface : MonoBehaviour
{
    public bool IsConnectedToPower; // another layer of control (if object i snot connected to power than even if you request for SetMode(true) it take no effect
    public float ActiveCooldown; // -1 means stay in active state forever
    public float NonActiveCooldown; // -1 means stay in non active state forever
    public bool InitOnAwake;

    private float _currentCooldownTimer;

    private Material _activeMaterial;
    private Material _nonActiveMaterial;
    private Renderer _renderer;
    private Node _node;
    private bool _activeState;

    void Reset()
    {
        InitDefault();
        InitOnAwake = true;
    }

    void Awake()
    {
        if (InitOnAwake)
            Init();
    }

    private void Init()
    {
        _node = GetComponent<Node>();
        _renderer = GetComponent<Renderer>();
        _activeMaterial = _renderer.sharedMaterial;
        _nonActiveMaterial = GamePrefabs.Instance.Materials["Gray"];
        _currentCooldownTimer = -1f; // it always stuck on initial state
        Assert.IsTrue(HasPowerEffect(), "you have attach ChargeSurfaceCooldown only to node with NodeProps.ChargeSurface");
        _activeState = HasPowerEffect();
        PowerEffect(IsConnectedToPower);
    }

    public void InitDefault()
    {
        ActiveCooldown = -1f;
        NonActiveCooldown = 0.15f;
        IsConnectedToPower = true;
        if (Application.isPlaying) // for reset
            Init();
    }

    void Update()
    {
        // update cooldowns
        if (_currentCooldownTimer >= 0f) // for -1 (infinity) we never update
        {
            _currentCooldownTimer -= Time.deltaTime;

            if (_currentCooldownTimer < 0f)
            {
                ToggleSwitch(!_activeState);
            }
        }
    }

    public bool HasPowerEffect()
    {
        return _node.NodeProps.HasFlag(NodeProps.ChargeSurface);
    }

    // energy support
    public void SetPower(bool flag)
    {
        IsConnectedToPower = flag;
        if (!IsConnectedToPower) // power off
        {
            if (_activeState)
                PowerEffect(false);
        }
        else // power on 
        {
            if (_activeState)
                PowerEffect(true);
        }
    }

    public void ToggleSwitch(bool activeState, bool ignoreCooldown = false)
    {
        _activeState = activeState;

        if (ignoreCooldown || !IsConnectedToPower)
            _currentCooldownTimer = -1f;
        else
            _currentCooldownTimer = _activeState ? ActiveCooldown : NonActiveCooldown;

        if (_activeState)
        {
            if (!IsConnectedToPower)
                return;

            Assert.IsTrue(!HasPowerEffect());
            _node.NodeProps |= NodeProps.ChargeSurface;
            _renderer.sharedMaterial = _activeMaterial;
        }
        else // disable
        {
            Assert.IsTrue(HasPowerEffect());
            _node.NodeProps &= ~NodeProps.ChargeSurface;
            _renderer.sharedMaterial = _nonActiveMaterial;
        }
    }

    public void PowerEffect(bool on)
    {
        if (on)
        {
            _node.NodeProps |= NodeProps.ChargeSurface;
            _renderer.sharedMaterial = _activeMaterial;
        }
        else // disable
        {
            _node.NodeProps &= ~NodeProps.ChargeSurface;
            _renderer.sharedMaterial = _nonActiveMaterial;
        }
    }

    [ContextMenu("SetPower(true)")]
    void DbgSetPowerTrue()
    {
        SetPower(true);
    }

    [ContextMenu("SetPower(false)")]
    void DbgSetPowerFalse()
    {
        SetPower(false);
    }
}
