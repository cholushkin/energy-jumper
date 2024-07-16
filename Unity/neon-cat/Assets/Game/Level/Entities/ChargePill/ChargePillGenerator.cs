using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class ChargePillGenerator : MonoBehaviour
{
    public Pickable ActivePill;
    public Pickable PillPrefab;
    public float Radius;
    public float DetectRadius;
    public float SpawnNextDelay;
    private Rigidbody _rb;
    public Transform Pointer;
    public ChargePillGeneratorVisual Visual;
    private float _spawnDelay = -1;

    void Awake()
    {
        _rb = ActivePill.GetComponent<Rigidbody>();
        Visual.PlayIdleAnimation();
        ActivePill.OnPickup += OnPickUpPill;
    }

    void Update()
    {
        if (_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
            Visual.CooldownProgress(Mathf.Clamp01(_spawnDelay / SpawnNextDelay));
            if (_spawnDelay <= 0f)
                SpawnPill();
        }
    }

    private void SpawnPill()
    {
        Assert.IsNull(ActivePill);
        Visual.PlaySpawnAnimation();
        ActivePill = Instantiate(PillPrefab, transform);
        ActivePill.transform.localPosition = Vector3.zero;
        ActivePill.OnPickup += OnPickUpPill;
        ActivePill.GetComponent<Follower>().Target = Pointer;
    }

    void FixedUpdate()
    {
        if (!MovePointer())
        {
            // we are moving to the center
        }
        else
        {
            // we are moving to the player
        }
    }

    private bool MovePointer()
    {
        Pointer.position = transform.position;
        var player = StateGameplay.Instance.Player;
        if (player == null)
            return false;

        var playerToCenterDistance = Vector3.Distance(player.transform.position, transform.position);
        if (playerToCenterDistance > DetectRadius)
            return false;

        var factor = Mathf.Clamp01(playerToCenterDistance / DetectRadius); // 0 - center; 1 - on the radius
        var revFactor = 1f - factor; // 1 - center; 0 - on the radius

        if (playerToCenterDistance < Radius)
            Pointer.position = player.transform.position;
        else
            Pointer.position = transform.position + (player.transform.position - transform.position).normalized * Radius * revFactor;
        return true;
    }

    private void OnPickUpPill(Pickable obj)
    {
        Visual.PlayPickupAnimation();
        _spawnDelay = SpawnNextDelay;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, Radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        if (Pointer != null)
            Gizmos.DrawSphere(Pointer.position, 0.5f);
    }
}
