using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PulseGrenade : MonoBehaviour
{
    [SerializeField] private PulseGrenadePulse pulsePrefab;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new Collider collider;
    private PulseGrenadeSettings settings;

    private LayerMask activateOnCollideWith;
    private bool activated;

    public void Set(PulseGrenadeSettings settings, Vector3 forceDirection)
    {
        activateOnCollideWith = settings.activateOnCollideWith;
        rb.AddForce(forceDirection * settings.shootForce);
        this.settings = settings;
        StartCoroutine(Lifetime());
    }

    private List<PulseGrenadePulse> spawned = new List<PulseGrenadePulse>();

    private IEnumerator Lifetime()
    {
        yield return new WaitUntil(() => activated);

        for (int i = 0; i < settings.pulses; i++)
        {
            PulseGrenadePulse spawned = Instantiate(pulsePrefab, transform);
            this.spawned.Add(spawned);
            spawned.Set(settings, delegate
            {
                Destroy(spawned.gameObject);
                this.spawned.Remove(spawned);
            });

            if (i != settings.pulses - 1)
                yield return new WaitForSeconds(settings.timeBetweenPulses);
        }

        yield return new WaitUntil(() => spawned.Count == 0);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, activateOnCollideWith)) return;
        activated = true;

        if (settings.isSticky)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;

            collider.enabled = false;

            transform.SetParent(collision.transform, true);

            if (collision.transform.TryGetComponent(out EndableEntity endableEntity))
            {
                endableEntity.AddAdditionalOnEndAction(delegate
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;

                    collider.enabled = true;

                    transform.SetParent(null);
                });
            }
        }
    }
}
