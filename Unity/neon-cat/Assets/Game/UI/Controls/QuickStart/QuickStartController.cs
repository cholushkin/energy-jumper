using UnityEngine;

public class QuickStartController : MonoBehaviour
{
    public float HideDelay;
    public QuickStartView View;
    private float _hiddenTime;
    private int _level;

    public void Awake()
    {
        View.Hide();
    }

    public void ProccessHiding()
    {
        _hiddenTime = HideDelay;
    }

    public void Update()
    {
        _hiddenTime -= Time.deltaTime;

        if (View.IsHidden)
        {
            if(_hiddenTime <= 0f)
                View.PlayRevealAnimation();
        }
        else // is not hidden
        {
            if (_hiddenTime > 0f)
                View.PlayHideAnimation();
        }
    }

    public void RechargeMultiClickPreventer()
    {
        View.Button.GetComponent<PreventMultipleClick>().Recharge();
    }

    public void SetLevel(int level)
    {
        _level = level;
        View.SetLevel(level);
    }

    public int GetLevel()
    {
        return _level;
    }
}
