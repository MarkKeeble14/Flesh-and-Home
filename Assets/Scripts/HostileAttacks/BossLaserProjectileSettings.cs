using UnityEngine;

[System.Serializable]
public struct BossLaserProjectileSettings
{
    public float Damage;
    public float HitForce;
    public float Speed;
    public LaserVisuals Visuals;
    public LayerMask CanHit;
    public LayerMask CanDamage;
}
