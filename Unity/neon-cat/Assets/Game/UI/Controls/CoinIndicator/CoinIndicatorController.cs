using GameLib.Alg;
using GameLib.Log;
using UnityEngine;

// Generic coin indicator controller
public class CoinIndicatorController : MonoBehaviour
{
    public IconIndicatorView View;
    public LogChecker Log;

    public void InitCoins(int coins)
    {
        if(Log.Verbose())
            Debug.Log($"{transform.GetDebugName()} InitCoins {coins}");
        View.InitValue(coins);
    }

    public void AddCoins(int coins)
    {
        if (Log.Verbose())
            Debug.Log($"{transform.GetDebugName()} AddCoins {coins}");
        View.ChangeValue(coins);
    }

    public void UpdateCoins(int coins)
    {
        if (Log.Verbose())
            Debug.Log($"{transform.GetDebugName()} UpdateCoins {coins}");
        if (View.GetValue() != coins)
            View.ChangeValue(coins - View.GetValue());
    }
}
