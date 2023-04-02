using System.Collections;
using UnityEngine;

public class BossFleshySpawnerOrb : MonoBehaviour
{
    [SerializeField] private BossFleshSpawn fleshPrefab;

    [SerializeField] private LayerMask spawnOn;

    [Header("Settings")]
    [SerializeField] private Vector2 depthOffsetRange;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private Vector2 minMaxScale;
    [SerializeField] private Vector2 minMaxGrowSpeed;
    [SerializeField] private Vector2 minMaxShrinkSpeed;
    [SerializeField] private float heightAllowance = 5f;

    public bool Enabled = true;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Start");
        StartCoroutine(SpawnBossFlesh());
    }

    private IEnumerator SpawnBossFlesh()
    {
        // Raycast to find where ground is
        RaycastHit hit;
        Vector3 r = Random.onUnitSphere;
        Vector3 pos = transform.position + (r * heightAllowance);
        Vector3 dir = transform.position - pos;
        Physics.Raycast(pos, dir, out hit, Mathf.Infinity, spawnOn);


        Vector3 spawnPos = hit.point
            + (r * RandomHelper.RandomFloat(depthOffsetRange));
        BossFleshSpawn spawn = Instantiate(fleshPrefab, spawnPos, Quaternion.identity, ClutterSavior._Instance.transform);
        spawn.Set(minMaxScale, minMaxGrowSpeed, minMaxShrinkSpeed);


        yield return new WaitForSeconds(timeBetweenSpawns);

        // Debug.Log("Wait");

        if (Enabled)
            StartCoroutine(SpawnBossFlesh());
    }
}
