using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceControl : MonoBehaviour
{
    public Follower IconWorldPivot;
    public Camera Camera;

    void Update()
    {
        var screenPos = Camera.WorldToScreenPoint(IconWorldPivot.transform.position);
        transform.position = screenPos;
    }
}
