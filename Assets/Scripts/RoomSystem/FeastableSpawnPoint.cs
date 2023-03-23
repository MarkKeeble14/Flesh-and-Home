using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeastableSpawnPoint : MonoBehaviour
{
    [Header("Emission Settings")]
    [SerializeField] private Vector3 addForceDirection;
    [SerializeField] private float addForceStrength;
    [SerializeField] private Vector3 randomOffsetRange = new Vector3(.25f, .25f, .25f);

    public GameObject Spawn(GameObject feastable, bool useOffset)
    {
        GameObject spawned = Instantiate(feastable, transform.position +
            (useOffset ? RandomHelper.RandomOffset(randomOffsetRange.x, randomOffsetRange.y, randomOffsetRange.z) : Vector3.zero), Quaternion.identity);

        if (spawned.TryGetComponent(out Rigidbody rb))
        {
            AddForce(rb);
        }
        return spawned;
    }

    public void AddForce(Rigidbody rb)
    {
        rb.AddForce(addForceDirection * addForceStrength);
    }
}
