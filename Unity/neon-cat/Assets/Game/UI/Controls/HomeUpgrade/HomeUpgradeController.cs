using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class HomeUpgradeController : MonoBehaviour
{
    public HomeIslandController HomeIsland;
    public HomeUpgradeView View;

    public void SetPrice(bool noAnimation = false)
    {
        var upgrade = HomeIsland.GetCurrentUpgrade();
        var price = upgrade?.PriceCoins ?? -1;
        if (price < 0) // special case when there is no more upgrades
        {
            View.SetInteractable(false);
            View.SetPriceInfoVisible(false, false);
            return;
        }

        View.SetInteractable(HomeIsland.AbleToDoUpgrade());
        View.SetPriceInfoVisible(price != 0, noAnimation); // special case - show no price for upgrade - hide number and small icon

        if(noAnimation)
            View.InitValue(price);
        else
            View.SetValue(price);
    }

    public void OnUpgradePress()
    {
        Assert.IsTrue(HomeIsland.AbleToDoUpgrade());
        HomeIsland.DoUpgrade();
        SetPrice();
    }

    void Update()
    {
        View.SetInteractable(HomeIsland.AbleToDoUpgrade());
    }
}
