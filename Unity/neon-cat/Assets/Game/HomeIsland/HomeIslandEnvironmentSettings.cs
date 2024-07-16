using Events;
using GameLib.Dbg;
using UnityEngine;

public class HomeIslandEnvironmentSettings : MonoBehaviour, IHandle<AspectRatioHelper.EventScreenOrientationChanged>
{
    public class CameraSettings
    {
        public float CameraDistancePortrait;
        public float CameraDistanceLandscape;
    }

    [Header("Environment color")]
    public Color AmbientSkyColor;
    public Color AmbientEquatorColor;
    public Color AmbientGroundColor;
    public Color SkyColor;
    public Color BloomTintColor;

    [Header("Fog parameters")] 
    public bool EnableFog;
    public Color FogColor;
    public float FogStartLandscape;// 100 
    public float FogEndLandscape; // 240
    public float FogStartPortrait;
    public float FogEndPortrait;


    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void Apply()
    {
        if(!Application.isPlaying)
            return;
        ApplyOrientationRelatedSettings();

        HomeCameraController.Instance.GetCamera().backgroundColor = SkyColor;
        GlobalVolume.Instance.GetBloom().tint.value = BloomTintColor;

        RenderSettings.ambientSkyColor = AmbientSkyColor;
        RenderSettings.ambientEquatorColor = AmbientEquatorColor;
        RenderSettings.ambientGroundColor = AmbientGroundColor;

        RenderSettings.fogColor = FogColor;
        RenderSettings.fog = EnableFog;
    }

    private void ApplyOrientationRelatedSettings()
    {
        var portrait = Screen.orientation == ScreenOrientation.Portrait ||
                       Screen.orientation == ScreenOrientation.PortraitUpsideDown;
#if UNITY_EDITOR
        portrait = Screen.width < Screen.height;
#endif

        Debug.Log($"Applying level settings for orientation {Screen.orientation}");
        if (portrait)
        {
            RenderSettings.fogStartDistance = FogStartPortrait;
            RenderSettings.fogEndDistance = FogEndPortrait;
        }
        else
        {
            RenderSettings.fogStartDistance = FogStartLandscape;
            RenderSettings.fogEndDistance = FogEndLandscape;
        }
    }


    public void Handle(AspectRatioHelper.EventScreenOrientationChanged message)
    {
        ApplyOrientationRelatedSettings();
    }
}
