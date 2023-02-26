using System.Collections;
using UnityEngine;

public class BossSetLaserProjectileAttack : Attack
{
    [SerializeField] private Transform barrelSetHolder;
    private LaserBarrel[] set;

    [SerializeField] private BossLaserProjectileSettings settings;
    [SerializeField] private Vector2 minMaxTimeBetweenShots = new Vector2(.025f, .1f);

    private void Awake()
    {
        // Get Barrels
        set = barrelSetHolder.GetComponentsInChildren<LaserBarrel>();
    }

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        for (int i = 0; i < set.Length; i++)
        {
            LaserBarrel selected = set[i];
            selected.ShootLaser(settings.Visuals.GetEmmissiveColor(settings.Visuals.GetLerpedColor(i / set.Length)), settings.CanHit, settings.CanDamage, settings.Speed, settings.Damage, settings.HitForce);
            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenShots));
        }
    }
}
