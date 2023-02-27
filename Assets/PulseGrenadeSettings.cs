using UnityEngine;

[System.Serializable]
public struct PulseGrenadeSettings
{
    // Laser info
    public float damage;
    public float timeBetweenPulses;
    public int pulses;
    public float shootForce;
    public LayerMask activateOnCollideWith;
    public LayerMask canDamage;
    public float crosshairSpread;
    public float pulseSpeed;
    public float pulseRadius;
    public float pulseKnockbackForce;
    public float pulseStayDuration;
}
