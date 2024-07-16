using LeakyAbstraction;
using UnityEngine;
using UnityEngine.Assertions;

// follows player instead of being attached to the player because 2 triggers works pretty bad on ridgit body object (magnet triggers hits on other triggers
[RequireComponent(typeof(ForceField))]
public class CaptureMagnet : MonoBehaviour
{
    public class EventGotPickup // Spawned when CaptureMagnet got pickup item
    {
        public Pickable PickableItem;
    }
    public ForceField ForceField;
    public float CaptureSuckRadius;

    public void SetIgnoreCollision(SphereCollider otherCollider)
    {
        Physics.IgnoreCollision(GetComponent<SphereCollider>(), otherCollider);
    }

    public void SetEnabled(bool flag)
    {
        ForceField.SetEnabled(flag);
        enabled = flag;
    }

    void FixedUpdate()
    {
        var suckInCandadate = ForceField.ZoneAnalyzer.GetClosestToCenter(true);
        if (suckInCandadate == null)
            return;
        var distance = Vector3.Distance(suckInCandadate.transform.position, transform.position);
        if (distance < CaptureSuckRadius)
            StartSucking(suckInCandadate); // pickable will disable physics 
    }

    private void OnGotPickupItem(Pickable item)
    {
        Assert.IsNotNull(item);
        SoundManager.Instance.PlaySound(GameSound.SfxPickupCoin);
        GlobalEventAggregator.EventAggregator.Publish(new EventGotPickup { PickableItem = item });
    }

    private void StartSucking(GameObject agent) 
    {
        print($"start sucking {agent.name}");
        Assert.IsNotNull(agent);
        Pickable p = agent.GetComponent<Pickable>();
        Assert.IsNotNull(p);
        Assert.IsFalse(p.IsInSucking());
        p.OnStartSuckIn(transform, 0f, () => FinishSucking(p));
    }

    private void FinishSucking(Pickable p)
    {
        OnGotPickupItem(p);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, CaptureSuckRadius);
    }
}
