using System.Collections.Generic;
using System.Linq;
using GameGUI;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

public class ControlPanelView 
    : MonoBehaviour
    , SimpleGUI.IInitialize
{
    public Transform[] Slots;

    private Dictionary<string, bool> _controlStates;

    public void Initialize()
    {
        _controlStates = new Dictionary<string, bool>();

        // initialize control states
        foreach (var slot in Slots) // for each slot
            foreach (Transform controlTransform in slot.transform) // for each control inside slot
            {
                Assert.IsFalse(_controlStates.ContainsKey(controlTransform.name));
                _controlStates[controlTransform.name] = controlTransform.gameObject.activeSelf;
            }
        Clear();
    }

    public void SyncTo(string[] revealingNames)
    {
        // for each slot
        foreach (var slot in Slots) 
        {
            // for each control inside slot
            foreach (Transform controlTransform in slot.transform)
            {
                var needToReveal = revealingNames.FirstOrDefault(x => controlTransform.gameObject.name == x) != null;
                if (needToReveal)
                {
                    if (_controlStates[controlTransform.name] == false)
                    {
                        PlayAppearAnimation(controlTransform);
                    }
                    controlTransform.GetComponent<PreventMultipleClick>()?.Recharge();
                }
                else
                {
                    if (_controlStates[controlTransform.name])
                    {
                        PlayHideAnimation(controlTransform);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        foreach (var slot in Slots) // for each slot
            foreach (Transform controlTransform in slot.transform) // for each control inside slot
            {
                controlTransform.gameObject.SetActive(false);
                _controlStates[controlTransform.gameObject.name] = false;
            }
    }

    private void PlayHideAnimation(Transform control)
    {
        Assert.IsNotNull(control);
        _controlStates[control.name] = false;
        control.GetComponent<SideReveal>().ForceComplete();
        control.GetComponent<SideHide>().Play();
        
    }

    private void PlayAppearAnimation(Transform control)
    {
        Assert.IsNotNull(control);
        _controlStates[control.name] = true;
        control.GetComponent<SideHide>().ForceComplete();
        control.GetComponent<SideReveal>().Play();
        
    }
}
