using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseGrenadePulse : MonoBehaviour
{
    private PulseGrenadeSettings settings;
    private List<Collider> hasHitList = new List<Collider>();

    private bool hasStarted;

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, settings.canDamage)) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(settings.damage, settings.pulseKnockbackForce * (other.transform.position - transform.position).normalized);
            hasHitList.Add(other);
        }
    }

    public void Set(PulseGrenadeSettings settings, Action onEnd)
    {
        if (hasStarted) return;
        hasStarted = true;

        this.settings = settings;
        StartCoroutine(Pulse(onEnd));
    }

    private IEnumerator Pulse(Action onEnd)
    {
        Vector3 targetScale = Vector3.one * settings.pulseRadius;

        for (float t = 0; t < settings.pulseStayDuration; t += Time.deltaTime)
        {
            if (transform.localScale.x >= settings.pulseRadius)
            {
                break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, settings.pulseSpeed * Time.deltaTime);
            yield return null;
        }

        /*
        transform.localScale = targetScale;

        yield return new WaitForSeconds(2f);
        */

        onEnd?.Invoke();
    }
}
