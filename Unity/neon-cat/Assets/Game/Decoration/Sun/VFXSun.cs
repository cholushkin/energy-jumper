using DG.Tweening;
using UnityEngine;

public class VFXSun : MonoBehaviour
{

    public Transform DestinationPoint;
    public Color DestinationColor;
    public float MoveDuration;
    private Material _sunMaterial;
    private const string BaseColorParameter = "_BaseColor";


    void Start()
    {
        _sunMaterial = GetComponent<Renderer>().material;
        StartSunAnimation();
    }

    private void StartSunAnimation()
    {
        transform.DOMove(DestinationPoint.position, MoveDuration).SetEase(Ease.OutCubic);
        transform.DOScale(transform.localScale * 1.2f, MoveDuration).SetEase(Ease.OutCubic);
        if (!_sunMaterial.HasProperty(BaseColorParameter))
            Debug.LogError($"no parameter {BaseColorParameter}");
        else
        {

            DOTween.To(() => _sunMaterial.GetColor(BaseColorParameter),
                    x => _sunMaterial.SetColor(BaseColorParameter, x), DestinationColor, MoveDuration)
                .SetEase(Ease.OutCubic);
        }
    }

}
