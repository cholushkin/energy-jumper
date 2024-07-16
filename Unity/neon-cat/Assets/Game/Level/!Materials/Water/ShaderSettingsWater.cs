using UnityEngine;

public class ShaderSettingsWater : MonoBehaviour
{
    public MeshRenderer WaterRenderer;

    public float WaterDepth;
    public Color ShallowWater;
    public Color DeepWater;
    public float RefractionSpeed;
    public float RefractionScale;
    public float RefractionStrength;
    public float FoamAmount;
    public float FoamCutOff;
    public float FoamSpeed;
    public float FoamScale;
    public Color FoamColor;

    public bool ApplySettingOnAwake;

    void Reset()
    {
        SetDefaultValue();
    }

    void Awake()
    {
        if (ApplySettingOnAwake)
        {
            var mat = new Material(WaterRenderer.sharedMaterial);
            WaterRenderer.sharedMaterial = mat;
            SetToMaterial();
        }
    }

    public void SetToMaterial()
    {
        var mat = WaterRenderer.sharedMaterial;
        
        mat.SetFloat("_WaterDepth", WaterDepth);
        mat.SetColor("_ShallowWater", ShallowWater);
        mat.SetColor("_DeepWater", DeepWater);
        mat.SetFloat("_RefractionSpeed", RefractionSpeed);
        mat.SetFloat("_RefractionScale", RefractionScale);
        mat.SetFloat("_RefractionStrength", RefractionStrength);
        mat.SetFloat("_FoamAmount", FoamAmount);
        mat.SetFloat("_FoamCutOff", FoamCutOff);
        mat.SetFloat("_FoamSpeed", FoamSpeed);
        mat.SetFloat("_FoamScale", FoamScale);
        mat.SetColor("_FoamColor", FoamColor);
    }

    public void SetFromMaterial()
    {
        var mat = WaterRenderer.sharedMaterial;
        WaterDepth = mat.GetFloat("_WaterDepth");
        ShallowWater = mat.GetColor("_ShallowWater");
        DeepWater = mat.GetColor("_DeepWater");
        RefractionSpeed = mat.GetFloat("_RefractionSpeed");
        RefractionScale = mat.GetFloat("_RefractionScale");
        RefractionStrength = mat.GetFloat("_RefractionStrength");
        FoamAmount = mat.GetFloat("_FoamAmount");
        FoamCutOff = mat.GetFloat("_FoamCutOff");
        FoamSpeed = mat.GetFloat("_FoamSpeed");
        FoamScale = mat.GetFloat("_FoamScale");
        FoamColor = mat.GetColor("_FoamColor");
    }

    public void SetDefaultValue()
    {
        WaterRenderer = GetComponent<MeshRenderer>();
        WaterDepth = 50f;
        ShallowWater = new Color32(136, 191, 255, 153);
        DeepWater = new Color32(88, 88, 255, 153);
        RefractionSpeed = 0.05f;
        RefractionScale = 2f;
        RefractionStrength = 0.07f;
        FoamAmount = 20f;
        FoamCutOff = 1.16f;
        FoamSpeed = 0.5f;
        FoamScale = 108.32f;
        FoamColor = new Color32(255, 255, 255, 181);
    }

    public void SetMaterialToDefault()
    {
        var waterDepth = 50f;
        var shallowWater = new Color32(136, 191, 255, 153);
        var deepWater = new Color32(88, 88, 255, 153);
        var refractionSpeed = 0.05f;
        var refractionScale = 2f;
        var refractionStrength = 0.07f;
        var foamAmount = 20f;
        var foamCutOff = 1.16f;
        var foamSpeed = 0.5f;
        var foamScale = 108.32f;
        var foamColor = new Color32(255, 255, 255, 181);

        var mat = WaterRenderer.sharedMaterial;
        mat.SetFloat("_WaterDepth", waterDepth);
        mat.SetColor("_ShallowWater", shallowWater);
        mat.SetColor("_DeepWater", deepWater);
        mat.SetFloat("_RefractionSpeed", refractionSpeed);
        mat.SetFloat("_RefractionScale", refractionScale);
        mat.SetFloat("_RefractionStrength", refractionStrength);
        mat.SetFloat("_FoamAmount", foamAmount);
        mat.SetFloat("_FoamCutOff", foamCutOff);
        mat.SetFloat("_FoamSpeed", foamSpeed);
        mat.SetFloat("_FoamScale", foamScale);
        mat.SetColor("_FoamColor", foamColor);
    }
}