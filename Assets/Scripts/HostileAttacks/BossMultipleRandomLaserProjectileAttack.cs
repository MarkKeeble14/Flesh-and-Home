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
        List<LaserBarrel> callOn = new List<LaserBarrel>();

        // Picking a random number of random barrels
        // Should potentially be a while loop so as to guarentee the maximum number of lasers being called; but dealing with the overlap of finding ones
        // which are unused seems not worthwhile for now at least
        for (int i = 0; i < toCall; i++)
        {
            LaserBarrel selected = options[Random.Range(0, options.Count)];

            // if a barrel has already been selected, ignore it
            if (!callOn.Contains(selected))
            {
                callOn.Add(selected);
            }
        }

        for (int i = 0; i < callOn.Count; i++)
        {
            if (executionInterrupted) break;

            LaserBarrel selected = callOn[i];
            selected.ShootLaser(settings.Visuals.GetEmmissiveColor(settings.Visuals.GetLerpedColor(i / callOn.Count)), settings.CanHit, settings.CanDamage, settings.Speed, settings.Damage, settings.HitForce);
            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenShots));
        }
    }
}
