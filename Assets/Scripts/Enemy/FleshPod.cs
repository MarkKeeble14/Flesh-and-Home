using System.Collections;
using UnityEngine;

public class FleshPod : KillableEntity, IRoomContent
{
    [Header("Settings")]
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private float capacity;
    [SerializeField] private float capacityPerCritter = 1f;
    [SerializeField] private float minCapacity = 25f;
    [SerializeField] private FleshPodSpawn critter;
    [SerializeField] private Vector2 minMaxCanSpawnPerTick;
    [SerializeField] private float timeBetweenTicks;
    [SerializeField] private LayerMask ground;

    [SerializeField] private RoomEnemySettings roomEnemySettings;

    [Header("Animations")]
    [SerializeField] private Animator anim;

    [Header("Settle")]
    [SerializeField] private float settleSpeed;
    private bool shouldSettle;
    private Vector3 startScale;
    private Vector3 targetScale;

    [Header("References")]
    [SerializeField] private Transform visualComponent;
    private new Renderer renderer;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onTickClip;
    [SerializeField] private AudioClipContainer onEmptyClip;

    private new void Awake()
    {
        // Set
        capacity = maxCapacity;
        startScale = visualComponent.localScale;
        targetScale = startScale;

        renderer = visualComponent.GetComponent<Renderer>();
        roomEnemySettings.SetInactiveColors(renderer);
        acceptDamage = false;

        AttachToFloor();

        base.Awake();
    }

    private void Update()
    {
        // Settle to intended scale
        if (shouldSettle && visualComponent.localScale != targetScale)
        {
            visualComponent.localScale = Vector3.Lerp(visualComponent.localScale, targetScale, Time.deltaTime * settleSpeed);
        }
    }

    private IEnumerator SpawningTick()
    {
        // Allow the pod to scale naturally
        shouldSettle = true;

        yield return new WaitForSeconds(timeBetweenTicks);

        // Audio
        onTickClip.PlayOneShot(source);

        int toSpawn = Mathf.RoundToInt(Random.Range(minMaxCanSpawnPerTick.x, minMaxCanSpawnPerTick.y));
        for (int i = 0; i < toSpawn; i++)
        {
            // Spawn position is 
            // the middle of the pod + some random offset somewhere between the entire bounds of the object on the x and z axis, and then for the y axis its
            // somewhere between the middle of the pod and the top of the pod
            FleshPodSpawn spawned = Instantiate(
                critter,
                visualComponent.position +
                    new Vector3(
                        Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2),
                        Random.Range(0, visualComponent.localScale.y / 2),
                        Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2)
                    )
                , Quaternion.identity);

            // Set the spawned thingy
            spawned.OnSpawn(GameManager._Instance.PlayerTransform);

            // Subtract from capacity
            capacity -= capacityPerCritter;
            // Check if we're out, if so then we destroy
            if (capacity <= minCapacity)
            {
                // Audio
                Instantiate(tempSource).Play(onEmptyClip);

                // Call On End
                onEndAction();
            }
        }

        // Set New Target Scale
        float percent = capacity / maxCapacity;
        targetScale = percent * startScale;

        AttachToFloor();
        Animate();
        StartCoroutine(SpawningTick());
    }

    private void Animate()
    {
        // Prevent the pod from scaling naturally so that we can control solely in the Animation
        shouldSettle = false;

        // Trigger animation
        anim.SetTrigger("Pulse");
    }

    private void AttachToFloor()
    {
        // Reattatch to floor
        RaycastHit hit;
        Ray ray = new Ray(transform.position + visualComponent.transform.up, Vector3.down);
        Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
        Vector3 groundPos = hit.point + (Vector3.up * targetScale.y / 2);
        transform.position = groundPos;
        // Debug.Log(transform.parent.name + ", Flesh Pod Ground Pos: " + groundPos);
    }

    public void Activate()
    {
        roomEnemySettings.SetActiveColors(renderer);
        acceptDamage = true;
        StartCoroutine(SpawningTick());
    }
}
