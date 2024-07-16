using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ArrowCanvas : MonoBehaviour
{
    private Transform _target; // following target
    public Transform _player { get; private set; }
    public Camera _camera { get; private set; }

    public Image LineImg;
    public Image PointerImg;
    public Image ArrowBg;

    public float PointerSpeed; // speed to move along the path
    public float DumpingFactor; // speed for SmoothDump
    public float Duration; // -1 ininite
    public float CaptionDuration; // -1 ininite


    private float _currentDuration;
    private float _currentCaptionDuration;
    //private float _currentPath;


    private const float TargetOffset = 20f;
    private const float ArrowToPlayerOffset = 12f;
    private const float ScreenFrameMargin = 32f;
    private const float lineWidth = 5;

    private Vector3 _pointerPos;
    private Vector3 _velocity = Vector3.zero; // for SmoothDump

    void Start()
    {

    }

    void Update()
    {
        if(_target == null)
            return;

        Duration -= Time.deltaTime;

        UpdatePointer();
        if (PointerImg != null)
            PointerImg.transform.position = _pointerPos;
        UpdateLine();
        UpdateArrow();
    }

    public bool IsLifetimeEnded()
    {
        return Duration < 0f;
    }

    public void Set(Transform player, Transform target, float duration, float captionDuration, Camera camera)
    {
        //transform.localScale = Vector3.zero;
        _player = player;
        _target = target;
        Duration = duration;
        _currentDuration = duration;
        CaptionDuration = captionDuration;
        _camera = camera;
        //_currentPath = 0f;
    }

    void UpdatePointer()
    {
        // viewport space
        var T = _camera.WorldToViewportPoint(_target.position); // target point
        var P = _camera.WorldToViewportPoint(_player.position); // player point
        var TP = T - P; // tp direction vector

        // is inside the screen ?
        if (T.x >= 0f && T.x <= 1f && T.y >= 0f && T.y <= 1f)
        {
            _pointerPos = _camera.WorldToScreenPoint(_target.position);
            return;
        }

        // left-right sides
        var Pt1 = TP.magnitude * Mathf.Abs(Mathf.Clamp01(T.x) - P.x) / Mathf.Abs(P.x - T.x);

        // top-bottom sides
        var Pt2 = TP.magnitude * Mathf.Abs(Mathf.Clamp01(T.y) - P.y) / Mathf.Abs(P.y - T.y);
        _pointerPos = _camera.ViewportToScreenPoint(P + TP.normalized * Mathf.Min(Pt1, Pt2));
    }

    void UpdateLine()
    {
        var targetScreenPos = _camera.WorldToScreenPoint(_target.position);
        var playerScreenPos = _camera.WorldToScreenPoint(_player.position);
        var target2Player = targetScreenPos - playerScreenPos;
        var lineTransform = LineImg.transform as RectTransform;
        Assert.IsNotNull(lineTransform);
        lineTransform.sizeDelta = new Vector2(target2Player.magnitude, lineWidth);
        lineTransform.pivot = new Vector2(0, 0.5f);
        lineTransform.position = playerScreenPos;
        float angle = Mathf.Atan2(target2Player.y, target2Player.x) * Mathf.Rad2Deg;
        lineTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateArrow()
    {
        var playerToPointer = _pointerPos - _player.position;

        ArrowBg.transform.position = Vector3.SmoothDamp(
            ArrowBg.transform.position, _pointerPos,
            ref _velocity, DumpingFactor);
    }
}
