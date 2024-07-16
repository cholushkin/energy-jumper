using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarsBlockView : MonoBehaviour
{
    public Image[] Stars;
    public Image[] StarsPrev;

    public void Set(int newStars, int prevStars)
    {
        for (var i = 0; i < Stars.Length; i++)
        {
            Stars[i].gameObject.SetActive(i < newStars);
        }

        for (int i = 0; i < StarsPrev.Length; i++)
        {
            StarsPrev[i].gameObject.SetActive(i < prevStars);
        }
    }
}
