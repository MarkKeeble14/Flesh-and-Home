using UnityEngine;

[CreateAssetMenu(fileName = "PulseGrenadeSettings", menuName = "PulseGrenadeSettings")]
public class PulseGrenadeSettings : ScriptableObject
{
    // Laser info
    public float damage;
    public float timeBetweenPulses;
    public int pulses;
    public float shootForce;
    public LayerMask activateOnCollideWith;
    public LayerMask canDamage;
    public float crosshairSpread;
    public bool isSticky;
    public float pulseSpeed;
    public float pulseRadius;
    public float pulseKnockbackForce;
    public float pulseStayDuration;
    public bool pulseScaleOut;
    public float pulseScaleOutSpeed;
}
