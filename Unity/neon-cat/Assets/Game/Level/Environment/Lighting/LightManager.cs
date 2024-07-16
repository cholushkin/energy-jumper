//using Alg;
//using DG.Tweening;
//using UnityEngine;
//using UnityEngine.Assertions;


//// factors: 
//// * closer to DangerZone
//// * recharging
//// * damage / healing

//public class LightManager : Singleton<LightManager>
//{
//    public Gradient BgColors;
//    public float RechargeEnableSpeed;
//    public float RechargeDisableSpeed;

//    private float _targetRecharge;
//    private float _currentRecharge;



//    //public EntityJumper Entity;
//    public Light LightSource;
//    public Camera Camera;

//    //private float _currentIntencity;

//    protected override void Awake()
//    {
//        base.Awake();
//        _targetRecharge = 0;
//        _currentRecharge = 0f;
//    }

//    //void OnGUI()
//    //{
//    //    OnGuiHelper.Instance?.Left(6, string.Format("IsRecharged: {0} {1}", IsRecharged(), _currentRecharge));
//    //}

//    void Update()
//    {
//        UpdateBgColor();

//        // update recharge enabling\disabling
//        if (_targetRecharge == 1f && _currentRecharge < 1f)
//        {
//            _currentRecharge += Time.deltaTime * RechargeDisableSpeed;
//            if (_currentRecharge > 1f)
//                OnReachDisconnectedMode();
//        }
//        else if (_targetRecharge == 0f && _currentRecharge > 0f)
//        {
//            _currentRecharge -= Time.deltaTime * RechargeEnableSpeed;
//            if (_currentRecharge < 0f)
//                OnReachDisconnectedMode();
//        }
//    }

//    public void UpdateBgColor()
//    {
//        Camera.backgroundColor = BgColors.Evaluate(Mathf.Clamp01(_currentRecharge));
//    }

//    public bool IsRecharged()
//    {
//        return !(_currentRecharge > 1f);
//    }

//    public void SetRechargeInstantState(bool isConnected)
//    {
//        _targetRecharge = isConnected ? 0 : 1;
//        Assert.IsTrue(_targetRecharge == 1f || _targetRecharge == 0f);
//    }

//    void OnReachConnectedMode()
//    {
//        // todo: play sfx
//        // todo: play blink effect & particles
//        Debug.Log("Connected");
//        DoIntensityBlink();
//    }

//    void OnReachDisconnectedMode()
//    {
//        // todo: play sfx
//        // todo: play blink effect & particles
//        Debug.Log("Disconnected");
//        DoIntensityBlink();
//    }

//    void DoIntensityBlink()
//    {
//        //LightSource.DOIntensity(intencity, 1f).SetUpdate(true);
//    }
//}
