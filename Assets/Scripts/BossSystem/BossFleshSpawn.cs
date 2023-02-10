using System.Collections;
using UnityEngine;

public class BossFleshSpawn : MonoBehaviour
{
    private Vector3 scaleTarget;
    private float growSpeed;
    private float shrinkSpeed;
    public void Set(Vector2 minMaxScale, Vector2 minMaxGrowSpeed, Vector2 minMaxShrinkSpeed)
    {
        // Set Scale Target
        float randomScale = Random.Range(minMaxScale.x, minMaxScale.y);
        scaleTarget = randomScale * Vector3.one;

        // Set other properties
        growSpeed = Random.Range(minMaxGrowSpeed.x, minMaxGrowSpeed.y);
        shrinkSpeed = Random.Range(minMaxShrinkSpeed.x, minMaxShrinkSpeed.y);

        // Start Growing
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        while (transform.localScale.x < scaleTarget.x)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, scaleTarget, growSpeed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
