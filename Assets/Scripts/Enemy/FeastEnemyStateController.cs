using UnityEngine;

public class FeastEnemyStateController : RoomEnemyStateController
{
    [Header("Settings")]
    [SerializeField] private float scaleSpeed = 1f;
    public Vector3 TargetScale { get; set; }

    [Header("Detection")]
    [SerializeField] private float feastableDetectionRange = 12.5f;
    private Collider[] feastablesInRange;
    [SerializeField] private LayerMask feastable = 1 << 19;
    public bool HasRoomWideFeastableDetection { get; set; }
    public FeastableEntity CurrentTarget { get; private set; }
    public int FeastLevel { get; set; }

    private new void Awake()
    {
        TargetScale = transform.localScale;
        base.Awake();
    }

    private void SetCurrentTarget(FeastableEntity target)
    {
        // Debug.Log(name + ": Setting Current Target to: " + target);

        CurrentTarget = target;
        CurrentTarget.TargetedBy = this;
    }

    public void FindNewTarget()
    {
        // Try to find feastable to target
        // if enemy can find any spawned feastable in it's room, allow it to do so
        if (HasRoomWideFeastableDetection)
        {
            // if there are no spawned feastables in the room, return
            if (SpawnRoom.TotalRoomFeastables.Count == 0) return;

            foreach (FeastableEntity feastable in SpawnRoom.TotalRoomFeastables)
            {
                if (feastable.TargetedBy != null) continue;

                SetCurrentTarget(feastable);

                return;
            }
        }
        else
        {
            // Otherwise, detect in radius around enemy
            feastablesInRange = Physics.OverlapSphere(transform.position, feastableDetectionRange, feastable);

            if (feastablesInRange.Length == 0) return;

            Collider searchingForTarget = null;
            foreach (Collider potentialTarget in feastablesInRange)
            {
                // Debug.Log(name + " Checking: " + potentialTarget);
                if (potentialTarget.gameObject == gameObject)
                {
                    // Debug.Log(name + " Target Was Self; Continue");
                    continue;
                }
                else
                {
                    searchingForTarget = potentialTarget;
                    // Debug.Log(name + " Selected Target: " + target + "; Breaking");
                    break;
                }
            }

            // if no valid targets were found (none in range), return
            if (searchingForTarget == null) return;

            // assume that the target is a feastable entity, since any object that can be targeted should be a feastable entity
            // if this is not the case, there is user error somewhere
            FeastableEntity feastableTarget = searchingForTarget.GetComponent<FeastableEntity>();

            // if the chosen target is already being targeted, return
            if (feastableTarget.TargetedBy != null) return;

            // Set current target and feastable target data
            SetCurrentTarget(feastableTarget);

            // Debug.Log(name + " Permitted to Feast On " + target.name + ", Now Feast Locked");
        }
    }

    private new void Update()
    {
        base.Update();

        // Update Scale
        if (transform.localScale != TargetScale)
            transform.localScale = Vector3.Lerp(transform.localScale, TargetScale, scaleSpeed * Time.deltaTime);
    }
}
