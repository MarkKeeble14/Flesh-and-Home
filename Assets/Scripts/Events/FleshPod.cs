using System.Collections;
using UnityEngine;

public class FleshPod : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private float capacity;
    [SerializeField] private float capacityPerCritter = 1f;
    [SerializeField] private float minCapacity = 25f;
    [SerializeField] private FleshPodSpawn critter;
    [SerializeField] private Vector2 minMaxCanSpawnPerTick;
    [SerializeField] private float timeBetweenTicks;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private LayerMask ground;

    private void Awake()
    {
        // Set
        capacity = maxCapacity;
        startScale = transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(SpawningTick());
    }

    private IEnumerator SpawningTick()
    {
        yield return new WaitForSeconds(timeBetweenTicks);

        int toSpawn = Mathf.RoundToInt(Random.Range(minMaxCanSpawnPerTick.x, minMaxCanSpawnPerTick.y));
        for (int i = 0; i < toSpawn; i++)
        {
            FleshPodSpawn spawned = Instantiate(critter, transform.position + (Vector3.up * transform.localScale.y / 2), Quaternion.identity);
            spawned.OnSpawn();
            capacity -= capacityPerCritter;
            if (capacity <= minCapacity)
            {
                Destroy(gameObject);
            }
        }
        SetTransform();
        StartCoroutine(SpawningTick());
    }

    private void SetTransform()
    {
        // Set Scale
        float percent = capacity / maxCapacity;
        Vector3 newScale = percent * startScale;
        transform.localScale = newScale;
        Debug.Log(percent + ", " + newScale);

        // Reattatch to floor
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
        transform.position = hit.point + (Vector3.up * transform.localScale.y / 2);
    }
}
