using UnityEngine;
using UnityEngine.UI;

public class PagerView : MonoBehaviour
{
    public Button PageLeftButton;
    public Button PageRightButton;
    public Button[] PageButtons;
    public Sprite[] SelectionSprites;

    public void EnablePageLeftButton(bool flag)
    {
        PageLeftButton.interactable = flag;
    }

    public void EnablePageRightButton(bool flag)
    {
        PageRightButton.interactable = flag;
    }

    public void EnablePageButton(int index, bool flag)
    {
        PageButtons[index].interactable = flag;
    }

    public void SetPageSelector(int index)
    {
        for (int i = 0; i < PageButtons.Length; ++i)
        {
            PageButtons[i].gameObject.GetComponent<Image>().sprite = SelectionSprites[i == index ? 0 : 1];
        }
    }
}
