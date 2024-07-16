using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LevelDistanceProgressionBarView : MonoBehaviour
{
    public RectTransform ScaleRuler;
    public RectTransform DangerPointer;
    public RectTransform AgentPointerInstant;
    public RectTransform AgentPointer;
    public RectTransform TargetPointer;
    public RectTransform StartPointer;
    public TextMeshProUGUI LevelTextFrom;
    public TextMeshProUGUI LevelTextTo;
    public Image SlicedBar;

    public float PointerClampMin;
    public float PointerClampMax;

    public float SliceClampMin;

    private float _distance;


    public void SetLevelsTexts(int from, int to)
    {
        LevelTextFrom.text = (from + 1).ToString();
        LevelTextTo.text = (to + 1).ToString();
    }

    public void SetInitialPosition()
    {
        DangerPointer.anchoredPosition = Vector2.zero;
        AgentPointerInstant.anchoredPosition = Vector2.zero;
        AgentPointer.anchoredPosition = Vector2.zero;
        TargetPointer.anchoredPosition = Vector2.zero;
        StartPointer.anchoredPosition = Vector2.zero;
        _distance = (TargetPointer.localPosition - StartPointer.localPosition).magnitude;
        Assert.IsTrue(Math.Abs(_distance - 300f) < 0.01f);
    }

    public void SetProgression(float progression)
    {
        var pointerClampedPosX = Mathf.Clamp(_distance * progression, PointerClampMin, PointerClampMax);
        AgentPointer.anchoredPosition = new Vector2(pointerClampedPosX, AgentPointer.anchoredPosition.y);
        progression = Mathf.Clamp(progression, SliceClampMin, 1f);
        SlicedBar.fillAmount = progression;
    }
}