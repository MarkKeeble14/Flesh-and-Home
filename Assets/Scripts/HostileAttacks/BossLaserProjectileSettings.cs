using UnityEngine;

[System.Serializable]
public struct BossLaserProjectileSettings
{
    public float Damage;
    [SerializeField] private float damageBoost;
    public float HitForce;
    public float Speed;
    public LaserVisuals Visuals;
    public LayerMask CanHit;
    public LayerMask CanDamage;

    public void Boost()
    {
        Damage += damageBoost;
    }
}
