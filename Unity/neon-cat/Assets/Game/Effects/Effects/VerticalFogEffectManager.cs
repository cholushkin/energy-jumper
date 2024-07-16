using UnityEngine;

public class VerticalFogEffectManager : MonoBehaviour
{
    public Material FogMaterial;
    public Color TopColor;
    public Color BottomColor;
    public float Smoothness;
    public float Height;

    public Renderer[] FogRenderers;

    [SerializeField]
    [HideInInspector]
    private Material SingleInstance;


    void OnValidate()
    {
        if (SingleInstance == null)
            Reset();

        SingleInstance.SetColor("_TopColor", TopColor);
        SingleInstance.SetColor("_BottomColor", BottomColor);
        SingleInstance.SetFloat("_Smoothness", Smoothness);
        SingleInstance.SetFloat("_Height", Height);
    }

    void Reset()
    {
        FogRenderers = transform.GetComponentsInChildren<Renderer>(true);
        foreach (var fogRenderer in FogRenderers)
            ReplaceMaterialWithStaticInstance(fogRenderer);
    }


    private Material ReplaceMaterialWithStaticInstance(Renderer renderer)
    {
        if(SingleInstance == null)
            SingleInstance = new Material(FogMaterial);

        renderer.sharedMaterial = SingleInstance;
        return SingleInstance;
    }
}
