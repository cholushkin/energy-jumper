using UnityEngine;
using UnityEngine.UI;

public class BigButtonGeneric : MonoBehaviour
{
    public Image Image;
    public Sprite Normal;
    public Sprite Pressed;
    public Sprite Disabled;

    public void SetSpriteNormal() // called by animation event
    {
        Image.sprite = Normal;
    }

    public void SetSpritePressed() // called by animation event
    {
        Image.sprite = Pressed;
    }

    public void SetSpriteDisabled() // called by animation event
    {
        Image.sprite = Disabled;
    }
}
