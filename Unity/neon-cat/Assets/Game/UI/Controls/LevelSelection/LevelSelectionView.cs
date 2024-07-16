using System.Collections.Generic;
using CGTespy.UI;
using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelSelectionView : MonoBehaviour
{
    public SwipePages SwipePages;
    public List<PageView> PageViews { get; set; }
    public PageView SamplePage;


    void Awake()
    {
        CreatePages();
    }

    public void RecalculatePageAspects()
    {

    }

    public void SetPage(int page)
    {
        SwipePages.SetPage(page);
    }

    public void SyncLevelState(UserAccount.Data.LevelState levState)
    {
        // get icon controller
        var page = PageViews[levState.ChapterId];
        var level = page.Icons[levState.LevelId];
        level.SyncToState(levState);
    }

    private void CreatePages()
    {
        var controller = GetComponent<LevelSelectionController>();
        Assert.IsNotNull(controller);

        Assert.IsTrue(SwipePages.TotalPages == LevelManager.ChapterCount);
        var pageCount = SwipePages.TotalPages;
        PageViews = new List<PageView>(pageCount);
        PageViews.Add(SamplePage);
        for (int i = 1; i < pageCount; ++i)
        {
            var page = Instantiate(SamplePage);
            page.name = $"Page.{i:00}";
            page.Caption.text = $"Chapter {i+1}";
            page.transform.SetParent(SamplePage.transform.parent);
            page.transform.localScale = Vector3.one;
            var rt = page.transform as RectTransform;
            rt.SetAnchors(Vector3.zero, Vector3.one);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var offset = new Vector3(rt.Width() * i, 0, 0);
            rt.anchoredPosition = offset;

            page.CreateIcons(i,i * LevelManager.LevelsPerChapter, controller);
            PageViews.Add(page);
        }
        SamplePage.CreateIcons(0,0, controller);
    }
}
