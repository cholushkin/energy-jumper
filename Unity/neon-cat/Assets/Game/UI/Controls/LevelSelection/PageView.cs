using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageView : MonoBehaviour
{
    public LevelIconController LevelIconPrefab;
    public Transform[] Rows;
    public ScrollRect ScrollRect;
    public SwipePages SwipePages;
    public TextMeshProUGUI Caption;
    public Transform ScreenCenter;

    public LevelIconController[] Icons { get; set; }
    private const int MaxItemsPerRow = 4;


    void Start()
    {
        //Caption.SetParent(transform, true); // todo: recalculate size on EventScreenSizeChanged
    }


    public void Update()
    {
        // update caption transparency
        const float centerVisibleFramePercentOffset = 0.6f;
        var k = Mathf.Abs((transform.position - ScreenCenter.position).x / (Screen.width * 0.5f * centerVisibleFramePercentOffset));
        k = 1f - Mathf.Clamp01(k);
        Caption.color = new Color(Caption.color.r, Caption.color.g, Caption.color.b, k);


        //var isVerScroll = Mathf.Abs(ScrollRect.velocity.y) > 0f;
        //if(isVerScroll)
        //    SwipePages.BlockInput = true;
        //else
        //    SwipePages.BlockInput = false;

        ScrollRect.vertical = !SwipePages.IsInSwipe;
    }

    public void CreateIcons(int chapter, int startIndex, LevelSelectionController controller)
    {
        Icons = new LevelIconController[LevelManager.LevelsPerChapter];

        for (int i = 0; i < LevelManager.LevelsPerChapter; ++i)
        {
            var levIcon = Instantiate(LevelIconPrefab);
            levIcon.Init(chapter, startIndex + i);
            var rowIndex = i / MaxItemsPerRow;
            Icons[i] = levIcon;
            var rt = levIcon.GetComponent<RectTransform>();
            rt.SetParent(Rows[rowIndex]);
            rt.localScale = Vector3.one;

            // register click
            var i1 = i;
            levIcon.GetComponent<Button>().onClick.AddListener(
                () => controller.OnLevelSelected(chapter, i1));

        }

        //Canvas.ForceUpdateCanvases();
        ScrollRect.Rebuild(CanvasUpdate.Layout);
    }
}
