using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EyeRoller))]
public class EyeRollerEditor : Editor
{
    void OnSceneGUI()
    {
        EyeRoller t = target as EyeRoller;

        Handles.color = new Color(1, 0, 0, 0.2f);

        var from = Quaternion.AngleAxis(t.BlindSectorRotation - t.BlindSectorAngle*0.5f, t.transform.forward) * t.transform.right;

        Handles.DrawSolidArc(t.transform.position, t.transform.forward, from,
            t.BlindSectorAngle,t.DetectRadius);
    }
}