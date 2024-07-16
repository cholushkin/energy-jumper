using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class HomeIslandUpgradeVisualizer : MonoBehaviour
{
    public void VisualizePart(GameObject node)
    {
        
        if (node.activeSelf)
        {
            Debug.Log($"Visualize appearing {node.name}");
            // special cases for appearing
            switch (node.name)
            {
                case "MainIsland.001":
                case "InitialRock":
                case "[Switch0]RockNature":
                {
                    // don't do visual effect
                    Debug.Log("Special case: no effect");
                    return;
                }
                case "[Switch1]RockTwoParts":
                {
                    Debug.Log("Special case: explosion");
                    DoExplosion(node);
                    return;
                }
                case "Ship":
                {
                    //Messenger.Instance.ShowMessage("On-board computer activated...Abnormal electric activity detected.");
                    return;
                }
                default:
                {
                    OnEnableObject(node);
                    return;
                }
            }
        }
        else
        {
            Debug.Log($"Visualize disappearing");
            // special cases for disappearing
            switch (node.name)
            {
                case "MainIsland.001":
                case "InitialRock":
                case "[Switch0]RockNature":
                {
                    // don't do visual effect
                    Debug.Log("Special case: no effect");
                    return;
                }
                default:
                {
                    OnDisableObject(node);
                    return;
                }
            }
        }
    }

    private void DoExplosion(GameObject node)
    {
        StartCoroutine(ExplosionCoroutine(node));
    }

    private void OnDisableObject(GameObject disabledGameObject)
    {
        EffectManager.Instance.ApplyHologramHideEffect(disabledGameObject, true);
    }

    private void OnEnableObject(GameObject enabledGameObject)
    {
        EffectManager.Instance.ApplyHologramRevealEffect(enabledGameObject, true);
    }



    #region custom scenarios implementation

    private IEnumerator ExplosionCoroutine(GameObject node)
    {
        var fractureController = node.GetComponentInChildren<FractureController>(true);
        Assert.IsNotNull(fractureController);
        fractureController.gameObject.SetActive(true);
        fractureController.Crack(Vector3.down, 1f, transform.position);
        yield return new WaitForSeconds(0.1f);
        fractureController.Crack(Vector3.down, 1f, transform.position);
        yield return new WaitForSeconds(0.1f);
        //fractureController.Explode(Vector3.up, 300f, transform.position, true);
        fractureController.Explode(Vector3.up, 500f, transform.position, true);
    }


    #endregion

}
