using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerController : MonoBehaviour
{
    public SwipePages Swiper;
    public PagerView View;



    public void OnPageLeftButtonPress()
    {
        Swiper.SetPage(Swiper.CurrentPage - 1, true);
    }

    public void OnPageRightButtonPress()
    {
        Swiper.SetPage(Swiper.CurrentPage + 1, true);
    }

    public void OnPagePress(int index)
    {
        Swiper.SetPage(index, true);
    }

    public void SyncToPage(int page)
    {
        View.SetPageSelector(page);
        
        View.EnablePageLeftButton(page != 0);
        View.EnablePageRightButton(page != Swiper.TotalPages-1);
    }
}
