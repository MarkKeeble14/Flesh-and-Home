using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMultipleRandomLaserProjectileAttack : Attack
{
    [Header("Specific Settings")]
    [SerializeField] private Transform[] barrelOptionHolders;
    private List<LaserBarrel> options = new List<LaserBarrel>();

    [SerializeField] private BossLaserProjectileSettings settings;
    [SerializeField] private int maxLaserCalls = 8;
    [SerializeField] private Vector2 minMaxTimeBetweenShots = new Vector2(.025f, .1f);
    [SerializeField] private bool aimAtPlayer;

    public override void Boost()
    {
        settings.Boost();
        base.Boost();
    }

    private void Awake()
    {
        // Fetch and add all options
        foreach (Transform holder in barrelOptionHolders)
        {
            options.AddRange(holder.GetComponentsInChildren<LaserBarrel>());
        }
    }

    protected override IEnumerator ExecuteAttack(Transform target)
    {
        int toCall = Random.Range(1, maxLaserCalls);

        for (int i = 0; i < toCall; i++)
        {
            if (executionInterrupted) break;

            LaserBarrel selected = options[Random.Range(0, options.Count)];
            selected.ShootLaser(settings.Visuals.GetEmmissiveColor(settings.Visuals.GetLerpedColor(i / toCall)),
                settings.CanHit, settings.CanDamage, settings.Speed, settings.Damage, settings.HitForce, aimAtPlayer);
            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenShots));
        }
    }
}
