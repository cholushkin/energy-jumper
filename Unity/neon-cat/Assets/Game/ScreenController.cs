using GameLib.Dbg;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public class EventScreenSizeChanged
    {
        public int Width;
        public int Height;
    }

    public int TargetFrameRate;
    public bool TrackScreenOrientationChange;
    public bool TrackScreenSizeChange;
    private ScreenOrientation _curScreenOrientation;
    private Vector2Int _curScreenSize;


    public void Awake()
    {
        Application.targetFrameRate = TargetFrameRate;
        _curScreenSize = new Vector2Int(Screen.width, Screen.height);
    }

    void Update()
    {
        if (TrackScreenOrientationChange && (_curScreenOrientation != Screen.orientation))
        {
            _curScreenOrientation = Screen.orientation;
            Debug.Log($"{Screen.orientation} {Screen.width} {Screen.height}");
            GlobalEventAggregator.EventAggregator.Publish(new AspectRatioHelper.EventScreenOrientationChanged());
        }

        if (TrackScreenSizeChange && (_curScreenSize.x != Screen.width || _curScreenSize.y != Screen.height))
        {
            GlobalEventAggregator.EventAggregator.Publish(new EventScreenSizeChanged{Width =  Screen.width, Height = Screen.height});
        }
    }
}
