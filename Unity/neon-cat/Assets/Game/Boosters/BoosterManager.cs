using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using GameLib.Alg;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    public enum Booster
    {
        Recharge
    }
    
    public void ActivateBooster( Booster booster )
    {
        if (booster == Booster.Recharge)
        {
            StateGameplay.Instance.Player?.EnableEnergy(true);
            return;
        }
        throw new NotImplementedException($"{booster}");
    }

    public int GetBoosterUseCost(Booster booster)
    {
        if (booster == Booster.Recharge)
            return 5;
        throw new NotImplementedException($"{booster}");
    }
}
