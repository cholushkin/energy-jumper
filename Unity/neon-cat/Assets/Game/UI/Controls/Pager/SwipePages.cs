using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipePages : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public class EventPageOpened
    {
        public int Page;
    }

    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public int TotalPages;

    public RectTransform ContentHolder;
    public int CurrentPage { get; private set; }
    public bool IsInSwipe { get; private set; }
    public bool BlockInput { get; set; }
    private Vector2 _beginAnimationPos;
    private Vector3 _contentHolderOriginPosition;


    void Awake()
    {
        panelLocation = ContentHolder.transform.position;
        _contentHolderOriginPosition = panelLocation;
    }

    void OnEnable()
    {
        panelLocation = ContentHolder.transform.position;
    }

    public void OnDrag(PointerEventData data)
    {
        if(BlockInput)
            return;
        float difference = data.pressPosition.x - data.position.x;
        float percent = difference / Screen.width;
        if (Mathf.Abs(percent) > 0.1f && !IsInSwipe)
        {
            // on enter swipe state
            IsInSwipe = true;
            _beginAnimationPos = data.position;
        }

        if (!IsInSwipe)
            return;

        // swipe animation processing
        float animDifference = _beginAnimationPos.x - data.position.x;
        ContentHolder.transform.position = panelLocation - new Vector3(animDifference, 0, 0);
    }


    public void OnEndDrag(PointerEventData data)
    {
        if (BlockInput)
            return;
        if (!IsInSwipe)
            return;
        // on exit swipe state
        IsInSwipe = false;
        float percentage = (_beginAnimationPos.x - data.position.x) / Screen.width;

        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && CurrentPage < TotalPages-1)
            {
                CurrentPage++;
                newLocation += new Vector3(-Screen.width, 0, 0);
                GlobalEventAggregator.EventAggregator.Publish(new EventPageOpened { Page = CurrentPage });
            }
            else if (percentage < 0 && CurrentPage > 0)
            {
                CurrentPage--;
                newLocation += new Vector3(Screen.width, 0, 0);
                GlobalEventAggregator.EventAggregator.Publish(new EventPageOpened { Page = CurrentPage });
            }

            panelLocation = newLocation;
            ContentHolder.transform.DOMove(newLocation, 1.0f).SetEase(Ease.OutQuint);
        }
        else
        {
            ContentHolder.transform.DOMove(panelLocation, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void SetPage(int page, bool swipeAnimation = false)
    {
        CurrentPage = page;
        panelLocation = _contentHolderOriginPosition + new Vector3(-Screen.width * page, 0, 0);
        if(swipeAnimation)
            ContentHolder.transform.DOMove(panelLocation, 0.5f).SetEase(Ease.InOutCirc);
        else
            ContentHolder.transform.position = panelLocation;
        GlobalEventAggregator.EventAggregator.Publish(new EventPageOpened{Page = page});
    }

   
}

