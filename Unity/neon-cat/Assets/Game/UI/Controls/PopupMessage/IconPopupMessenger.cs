using UnityEngine;

public class IconPopupMessenger : MonoBehaviour
{
    public IconPopupMessage PrefabMessage;

    public void ShowMessage(string text, float duration)
    {
        var item = Instantiate(PrefabMessage, transform);
        item.SetText(text);
        item.Duration = duration;
    }
}