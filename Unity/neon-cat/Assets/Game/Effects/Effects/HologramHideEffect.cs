using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

// supports _renderer == null for hierarchy applyed effect (when some objects could have no renderer on it but still need to count time)
public class HologramHideEffect : MonoBehaviour
{
    private Material _hologram;
    private Material _original;
    private Renderer _renderer;
    private int  _glitchesCount;
    private float explosionDuration = 0.5f;
    private float explosionDistance = 2f;


    void Start()
    {
        if (_renderer == null)
        {
            Debug.Log($"HologramHideEffect.Start on '{gameObject.name}' (NO RENDER mode)");
            DOVirtual.DelayedCall(10, Finish);
        }
        else
        {
            Debug.Log($"HologramHideEffect.Start on '{gameObject.name}'");
            _glitchesCount = Random.Range(1, 2);
            _hologram.SetFloat("Reveal", 1f);
            DoGlitch();
        }
    }

    private void DoGlitch()
    {
        _hologram.DOFloat(0f, "Reveal", 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetDelay(Random.Range(0.25f, 1.0f))
            .SetEase(Ease.InOutBounce)
            .OnComplete(() =>
            {
                if (--_glitchesCount <= 0)
                    DoExplosion();
                else
                    DoGlitch();
            });
    }

    private void DoExplosion()
    {
        _hologram.SetVector("Explosion", Vector3.zero);
        _hologram.DOVector(Vector3.one * explosionDistance, "Explosion", explosionDuration).SetEase(Ease.OutCubic);


        _hologram.DOFloat(0f, "Reveal", explosionDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(Finish);
    }

    public void Init(Material hologramMaterial)
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            Assert.IsTrue(_renderer.materials.Length == 1);

            // replace material mode
            _original = _renderer.material;
            _hologram = new Material(hologramMaterial);
            _renderer.material = _hologram;
        }
        gameObject.SetActive(true);
    }

    private void Finish()
    {
        gameObject.SetActive(false);
        Destroy(this);
        Debug.Log($"HologramHideEffect.Finish on '{gameObject.name}'");
    }
}
