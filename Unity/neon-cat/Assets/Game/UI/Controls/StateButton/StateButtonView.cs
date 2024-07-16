using System;
using UnityEngine;
using UnityEngine.UI;

public class StateButtonView : MonoBehaviour
{
    [Serializable]
    public class ButtonState
    {
        public string StateName;
        public Transform Image;
        public Transform Text;
    }

    public ButtonState[] States;
    public Button Button;

    public void SwitchState(string stateName)
    {
        foreach (var state in States)
        {
            var activate = state.StateName == stateName;
            state.Image.gameObject.SetActive(activate);
            state.Text.gameObject.SetActive(activate);
            if (activate)
            {
                Button.targetGraphic = state.Image.GetComponent<Image>();
            }
        }
    }
}
