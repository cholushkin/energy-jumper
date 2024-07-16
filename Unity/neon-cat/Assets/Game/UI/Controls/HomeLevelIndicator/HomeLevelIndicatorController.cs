using UnityEngine;

public class HomeLevelIndicatorController : MonoBehaviour
{
    public IconIndicatorView View;

    public void Setup( int level )
    {
        if (level == 0)
        {
            View.SetValueVisible(false, true);
        }
        else
        {
            View.SetValueVisible(true);
            View.InitValue(level);
        }
    }

    public void Upgrade()
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        var level = account.AccountData.IslandLevel;
        if (level == 0)
        {
            View.SetValueVisible(false, true);
        }
        else
        {
            View.SetValueVisible(true);
            View.InitValue(level);
        }
    }

    public void SetVisibility(bool flag)
    {
        gameObject.SetActive(flag);
    }
}
