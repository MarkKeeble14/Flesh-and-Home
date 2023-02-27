using System.Collections;
using UnityEngine;

public class PulseGrenade : MonoBehaviour
{
    [SerializeField] private PulseGrenadePulse pulsePrefab;
    [SerializeField] private Rigidbody rb;

    private LayerMask activateOnCollideWith;
    private bool activated;

    public void Set(PulseGrenadeSettings settings, Vector3 forceDirection)
    {
        activateOnCollideWith = settings.activateOnCollideWith;
        rb.AddForce(forceDirection * settings.shootForce);
        StartCoroutine(Lifetime(settings));
    }

    private IEnumerator Lifetime(PulseGrenadeSettings settings)
    {
        yield return new WaitUntil(() => activated);

        for (int i = 0; i < settings.pulses; i++)
        {
            PulseGrenadePulse spawned = Instantiate(pulsePrefab, transform.position, Quaternion.identity);
            spawned.Set(settings, () => Destroy(spawned.gameObject));

            if (i != settings.pulses - 1)
                yield return new WaitForSeconds(settings.timeBetweenPulses);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, activateOnCollideWith)) return;
        activated = true;
    }
}
