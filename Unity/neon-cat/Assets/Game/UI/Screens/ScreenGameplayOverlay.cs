using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;

public class ScreenGameplayOverlay : GUIScreenBase
{
    public ControlPanelController ControlPanel;

    public override void OnPopped()
    {
        base.OnPopped();
        ControlPanel.Clear();
    }

}
