using System;
using System.Collections.Generic;
using Events;
using Game;
using GameLib.Alg;
using GameLib.Dbg;
using UnityEngine;
using UnityEngine.UI;

public class LevelEnvironmentSettings : MonoBehaviour, IHandle<AspectRatioHelper.EventScreenOrientationChanged>
{
    [Serializable]
    public class OrientationSpecificSettings
    {
        public float CameraDistance;
        public float FogStart;
        public float FogEnd;
    }

    [Serializable]
    public class SideColors
    {
        public Color Front;
        public Color Back;
        public Color Top;
        public Color Bottom;
        public Color Left;
        public Color Right;
    }

    [Header("##### Orientation specific settings")]
    public OrientationSpecificSettings Landscape;
    public OrientationSpecificSettings Portrait;

    [Header("##### Environment color")]
    public Color AmbientColor;
    public Color SkyColor;
    public Color BloomTintColor;

    [Header("##### Unity Fog parameters")]
    public bool EnableFog;
    public Color FogColor;

    //[Header("##### Lushkin Fog parameters")] // another fog
    //public Color BottomFogColor;

    [Header("##### Level progress bar")]
    public bool ShowProgressbar;
    public Color CharacterPointerColor;
    public Color FillColor;
    public Color ScaleRulerColor;

    [Header("##### EnergyZone")]
    public Color EnergyFieldColor;
    public Color EnergyFieldBackgroundColor;

    [Header("##### Water")]
    public Color WaterCubeColor;

    [Header("##### Level and decorations")]
    public SideColors LevelBackgroundColors; // for material LevBackground
    public SideColors LevelCenterColors; // for material LevCenter
    public SideColors LevelForegroundColors; // for material LevForeground
    public SideColors DecorationNearColors; // for material LevDecorationNear
    public SideColors DecorationFarColors; // for material LevDecorationFar
    public SideColors DecorationForegroundColors; // for material LevDecorationForeground

    [Header("##### Physics")]
    public Vector3 Gravity;


    [Tooltip("Assigned on import, don't change it manually!")]
    public Sprite Thumbnail;
    private static readonly Dictionary<string, Material> materialInstances = new Dictionary<string, Material>(16);


    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void Reset()
    {
        Gravity = Vector3.down * 20f;
    }

    public void Apply()
    {
        ApplyOrientationRelatedSettings();

        Physics.gravity = Gravity;
        GameCamera.Instance.GetCamera().backgroundColor = SkyColor;

#if UNITY_EDITOR
        var thumbnailCamera = transform.Find("ThumbnailCamera")?.GetComponent<Camera>();
        if (thumbnailCamera != null)
        {
            var tmpRect = thumbnailCamera.rect;
            var tmpSize = thumbnailCamera.orthographicSize;
            var tmpPos = thumbnailCamera.transform.position;
            var tmpRot = thumbnailCamera.transform.rotation;
            var tmpScale = thumbnailCamera.transform.localScale;

            thumbnailCamera.CopyFrom(GameCamera.Instance.GetCamera());
            thumbnailCamera.orthographic = true;
            thumbnailCamera.orthographicSize = tmpSize;
            thumbnailCamera.transform.position = tmpPos;
            thumbnailCamera.transform.rotation = tmpRot;
            thumbnailCamera.transform.localScale = tmpScale;
            thumbnailCamera.rect = tmpRect;
        }
#endif

        GlobalVolume.Instance.GetBloom().tint.value = BloomTintColor;

        RenderSettings.ambientLight = AmbientColor;

        RenderSettings.fog = EnableFog;
        RenderSettings.fogColor = FogColor;


        var levelRenderers = transform.GetComponentsInChildren<Renderer>(true);
        ApplyMaterialToRenderers(levelRenderers);


        // ##### GUI
        if (StateGameplay.Instance.GameHUD)
        {
            StateGameplay.Instance.GameHUD.LevelProgressionController.gameObject.SetActive(ShowProgressbar);
            StateGameplay.Instance.GameHUD.LevelProgressionController.View
                .AgentPointer.GetComponent<Image>().color = CharacterPointerColor;
            StateGameplay.Instance.GameHUD.LevelProgressionController.View
                .ScaleRuler.GetComponent<Image>().color = ScaleRulerColor;
            StateGameplay.Instance.GameHUD.LevelProgressionController.View
                .SlicedBar.GetComponent<Image>().color = FillColor;
        }
    }

    public void ApplyMaterialToRenderers(Renderer[] renderers)
    {
        foreach (var renderer in renderers)
        {
            if (renderer.sharedMaterial == null)
            {
                Debug.LogWarning($"{renderer.transform.GetDebugName()} has no material");
                continue;
            }

            // ##### Energy zone
            var energyField = renderer.GetComponent<EnergyZone>();
            if (energyField != null)
            {
                SetColorShaderParam(ReplaceMaterialWithStaticInstance(energyField.Plasma, "LevEnergyZone"), "MainColor", EnergyFieldColor);
                SetColorShaderParam(ReplaceMaterialWithStaticInstance(energyField.Background, "LevEnergyZoneBg"), "_BaseColor", EnergyFieldBackgroundColor);
                continue;
            }

            // ##### Level and decorations
            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevCenter"), LevelCenterColors))
                continue;

            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevForeground"), LevelForegroundColors))
                continue;

            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevBackground"), LevelBackgroundColors))
                continue;

            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevDecorationNear"), DecorationNearColors))
                continue;

            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevDecorationFar"), DecorationFarColors))
                continue;

            if (SetColor6Sides(ReplaceMaterialWithStaticInstance(renderer, "LevDecorationForeground"), DecorationForegroundColors))
                continue;



            //// ##### Lushkin fog
            //if (SetColorShaderParam(ReplaceMaterialWithStaticInstance(renderer, "PerspectiveFogMaterial"), "FogColor", BottomFogColor))
            //    continue;

            // ##### Water cube
            if (SetColorCommon(ReplaceMaterialWithStaticInstance(renderer, "LevDeadlyWater"), WaterCubeColor))
                continue;

            // todo: set color for advanced water

            // other objects materials
            // ...
        }
    }

    private bool SetColorCommon(Material mat, Color color)
    {
        if (mat == null)
            return false;
        mat.color = color;
        return true;
    }

    private bool SetColor6Sides(Material mat, SideColors colors)
    {
        if (mat == null)
            return false;

        SetColorShaderParam(mat, "_ColorLeft", colors.Left);
        SetColorShaderParam(mat, "_ColorRight", colors.Right);
        SetColorShaderParam(mat, "_ColorFront", colors.Front);
        SetColorShaderParam(mat, "_ColorBack", colors.Back);
        SetColorShaderParam(mat, "_ColorTop", colors.Top);
        SetColorShaderParam(mat, "_ColorBottom", colors.Bottom);

        return true;
    }

    private bool SetColorShaderParam(Material mat, string paramName, Color color)
    {
        if (mat == null)
            return false;
        mat.SetColor(paramName, color);
        return true;
    }

    private Material ReplaceMaterialWithStaticInstance(Renderer renderer, string materialName)
    {
        if (renderer.sharedMaterial == null)
            return null;

        if (renderer.sharedMaterial.name.Equals(materialName))
        {
            Material mat = null;
            if (!materialInstances.TryGetValue(materialName, out mat))
            {
                mat = new Material(renderer.sharedMaterial);
                materialInstances.Add(materialName, mat);
            }
            renderer.sharedMaterial = mat;
            return mat;

        }
        return null;
    }


    private void ApplyOrientationRelatedSettings()
    {
        var portrait = Screen.orientation == ScreenOrientation.Portrait ||
                      Screen.orientation == ScreenOrientation.PortraitUpsideDown;
#if UNITY_EDITOR
        portrait = Screen.width < Screen.height;
#endif

        Debug.Log($"Applying level settings for orientation PORTRAIT:{portrait}");

        var settings = portrait ? Portrait : Landscape;

        GameCamera.Instance.DistanceMover.DistancePosition = new Vector3(0, 0, -settings.CameraDistance);
        RenderSettings.fogStartDistance = settings.FogStart;
        RenderSettings.fogEndDistance = settings.FogEnd;
    }

    public void Handle(AspectRatioHelper.EventScreenOrientationChanged message)
    {
        ApplyOrientationRelatedSettings();
    }
}
