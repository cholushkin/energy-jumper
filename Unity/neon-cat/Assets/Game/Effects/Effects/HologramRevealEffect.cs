using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class HologramRevealEffect : MonoBehaviour
{
    private Material _hologram;
    private Material _original;
    private Renderer _renderer;
    private int  _glitchesCount;
    private float firstRevealDuration = 0.5f;
    private float explosionDistance = 1.5f;


    void Start()
    {
        _glitchesCount = Random.Range(1, 2);
        if (_renderer != null)
        {
            Debug.Log($"HologramRevealEffect.Start on '{gameObject.name}'");
            _hologram.SetVector("Explosion", Vector3.one * explosionDistance);
            _hologram.DOVector(Vector3.zero, "Explosion", firstRevealDuration).SetEase(Ease.OutCubic);
            _hologram.SetFloat("Reveal", 0f);
            _hologram.DOFloat(1f, "Reveal", firstRevealDuration).OnComplete(DoGlitch);
        }
        else
        {
            Debug.Log($"HologramRevealEffect.Start on '{gameObject.name}' (NO RENDER mode)");
            DOVirtual.DelayedCall(10, Finish);
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
                    DoOriginalMaterial();
                else
                    DoGlitch();
            });
    }

    private void DoOriginalMaterial()
    {
        _hologram.DOFloat(0f, "Reveal", 0.1f)
            //.SetDelay(Random.Range(0.2f, 0.4f))
            .SetEase(Ease.InOutBounce)
            .OnComplete(() =>
            {
                _renderer.material = _original;
                Finish();
            });
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
    }

    private void Finish()
    {
        Destroy(this);
        Debug.Log($"HologramRevealEffect.Finish on '{gameObject.name}'");
    }
}
