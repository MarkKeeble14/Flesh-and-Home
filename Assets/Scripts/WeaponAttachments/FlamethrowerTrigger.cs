using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerTrigger : MonoBehaviour
{
    private FlamethrowerSettings.TriggerSettings settings;
    private bool active;
    private TimerDictionary<Collider> hitTickBetweenTimer = new TimerDictionary<Collider>();

    [Header("Visual")]
    [SerializeField] private bool showMesh;
    [SerializeField] private Color meshColor;

    [Header("References")]
    [SerializeField] private new Collider collider;
    [SerializeField] private new Renderer renderer;

    public void Activate(FlamethrowerSettings.TriggerSettings settings)
    {
        this.settings = settings;
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void Awake()
    {
        renderer.material.color = meshColor;
    }

    private void OnTriggerStay(Collider other)
    {
        // if not active, obviously we don't want to do anything
        if (!active) return;

        // if this object is not defined to be hit, return
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, settings.canHit))
        {
            // Debug.Log("Hit Something not in Mask: " + other);
            return;
        }

        // if we've hit this object too recently, return
        if (hitTickBetweenTimer.ContainsKey(other))
        {
            // Debug.Log("Has Already Hit: " + other);
            return;
        }

        // Debug.Log("Hitting: " + other.gameObject);

        // if other object has a component which implements IDamageable, use it to deal damage
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // Debug.Log("Damageing:" + damageable);
            // Debug.Log("Damaging: " + damageable);

            damageable.Damage(settings.damage, DamageSource.FLAMETHROWER);
            hitTickBetweenTimer.Add(other, settings.tickRate);
        }
    }

    private void Update()
    {
        // Update Dictionary
        hitTickBetweenTimer.Update();

        // Only have collider enabled if active
        collider.enabled = active;

        // Only show object if active and we wish to show mesh
        renderer.enabled = active && showMesh;
    }
}
