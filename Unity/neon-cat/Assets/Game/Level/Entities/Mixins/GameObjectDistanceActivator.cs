using UnityEngine;
using UnityEngine.Assertions;

public class GameObjectDistanceActivator : MonoBehaviour
{
    public enum Mode
    {
        OneShoot,
        Switcher,
    }

    public Mode WorkingMode;
    public float ActivationDistance;

    public GameObject Host; // object to disable\enalbe
    public Transform Target;  // object to analize distance

    void Reset()
    {
        ActivationDistance = 20f;
    }


    void Start()
    {
        Assert.IsNotNull(Target);
    }

    private void Update()
    {
        if(Target == null)
            return;

        var isInActivationZone = (Target.position - transform.position).magnitude < ActivationDistance;

        // activate
        if (isInActivationZone && Host.activeSelf == false)
        {
            Host.SetActive(true);
            return;
        }

        // deactivate
        if (!isInActivationZone && Host.activeSelf && WorkingMode == Mode.Switcher)
            Host.SetActive(false);
    }
}
