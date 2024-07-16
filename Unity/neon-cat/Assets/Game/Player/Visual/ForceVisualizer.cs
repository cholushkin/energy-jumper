using UnityEngine;

public class ForceVisualizer : MonoBehaviour
{
    public GameObject prefab;
    private GameObject[] dots;
    private const float _maxMagnitude = 12f;
    private const int dotsNumber = 10;
    private float _maxDotScale;
    private float _minDotScale;

    private int _dotsCount;
    private Vector3 _normalizedForce;
    private Vector3 _direction;



    void Awake()
    {
        _maxDotScale = prefab.transform.localScale.x;
        _minDotScale = _maxDotScale * 0.15f;
    }

    void Start()
    {
        dots = new GameObject[dotsNumber];
        for (int i = 0; dotsNumber != i; ++i)
        {
            var dot = Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
            dot.transform.parent = transform;
            dot.transform.localScale = Vector3.one * _minDotScale;
            dots[i] = dot;
            dot.SetActive(false);
        }
    }

    public void DisableDots()
    {
        Set(Vector3.zero, Vector3.zero);
    }

    public void Update()
    {
        const float pulseSpeed = 6f;
        const float pulsePart = 0.3f;
        var z = 0;
        float scaleStep = 0.3f;

        UpdateDots(_normalizedForce, _direction);

        for (int i = _dotsCount - 1; i >= 0; --i)
        {
            var thisStepScale = 1f + z++ * scaleStep;
            var pulseScale01 = (Mathf.Sin(i + Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
            var dotScale = _minDotScale * thisStepScale;
            dotScale = dotScale + (dotScale * pulsePart * pulseScale01);
            dots[i].transform.localScale = Vector3.one * dotScale;
        }
    }

    public void Set(Vector3 normalizedForce, Vector3 direction)
    {
        _normalizedForce = normalizedForce;
        _direction = direction;
    }

    private void UpdateDots(Vector3 normalizedForce, Vector3 direction)
    {
        normalizedForce = Vector3.ClampMagnitude(normalizedForce, 1f);
        const float voidRadius = 4f;
        var magnitude = _maxMagnitude * normalizedForce.magnitude;
        var lenPerDot = _maxMagnitude / dotsNumber;
        _dotsCount = (int)Mathf.Round(magnitude / lenPerDot);
        //print(normalizedForce.magnitude + "  " + _dotsCount);

        for (int i = 0; i < dotsNumber; i++)
        {
            dots[i].transform.position = transform.position + direction * (voidRadius + i * lenPerDot);
            dots[i].SetActive(i < _dotsCount);
        }
    }

    public void EnableDots()
    {
        //for (int i = 0; i < dotsNumber; ++i)
        //{
        //    dots[i].transform.position = transform.position;
        //    dots[i].SetActive(true);
        //}
    }
}
