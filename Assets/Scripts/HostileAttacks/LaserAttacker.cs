using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaserAttacker : Attacker
{
    [Header("Lasers")]
    [SerializeField] LaserAttackerOptions laserAttackerOptions;

    [Header("Barrel Sets")]
    [SerializeField] private Transform rotatingBarrelSetsHolder;
    [SerializeField] private Transform stationaryBarrelSetsHolder;
    protected BossBarrel[][] stationaryBarrelSets;
    protected BossBarrel[][] rotatingBarrelSets;

    protected BossPhaseManager bossPhaseManager;

    protected void Awake()
    {
        // Get Reference
        bossPhaseManager = FindObjectOfType<BossPhaseManager>();

        // Fetch and add all rotating sets
        rotatingBarrelSets = new BossBarrel[rotatingBarrelSetsHolder.childCount][];
        for (int i = 0; i < rotatingBarrelSetsHolder.childCount; i++)
        {
            Transform child = rotatingBarrelSetsHolder.GetChild(i);
            rotatingBarrelSets[i] = child.GetComponentsInChildren<BossBarrel>();
        }

        // Fetch and add all stationary sets
        stationaryBarrelSets = new BossBarrel[stationaryBarrelSetsHolder.childCount][];
        for (int i = 0; i < stationaryBarrelSetsHolder.childCount; i++)
        {
            Transform child = stationaryBarrelSetsHolder.GetChild(i);
            stationaryBarrelSets[i] = child.GetComponentsInChildren<BossBarrel>();
        }
    }

    private Vector3 GetLaserOrigin(LaserOriginType originType, BossBarrel barrel, Transform shell)
    {
        switch (originType)
        {
            case LaserOriginType.BARREL:
                return barrel.transform.position;
            case LaserOriginType.SHELL:
                return shell.position;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    private Vector3 GetDirectionOffset()
    {
        return new Vector3(
            Random.Range(laserAttackerOptions.MinMaxLaserOffset.x, laserAttackerOptions.MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1),
            Random.Range(laserAttackerOptions.MinMaxLaserOffset.x, laserAttackerOptions.MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1),
            Random.Range(laserAttackerOptions.MinMaxLaserOffset.x, laserAttackerOptions.MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1)
            );
    }

    protected IEnumerator LaserFrom(BossBarrel barrel, Transform shell)
    {
        if (barrel.IsFiring) yield break;

        if (barrel.Disabled) yield break;

        barrel.IsFiring = true;

        LineRenderer selected = barrel.LineRenderer;

        // New Dictionary
        TimerDictionary<Collider> laserHasHitDictionary = new TimerDictionary<Collider>();

        // Reset
        selected.widthMultiplier = 0;

        // Show
        selected.enabled = true;

        // Set Positions
        selected.positionCount = 2;

        // Grow Laser Width
        // Starting Up
        LaserAimingType aimingType = GetLaserAimingType();
        LaserOriginType originType = GetLaserOriginType(barrel);

        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 randomFocusPoint = randomDirection * laserAttackerOptions.LaserRange;
        Vector3 laserOrigin = GetLaserOrigin(originType, barrel, shell);
        Vector3 directionToPlayer = (bossPhaseManager.Player.position - laserOrigin + GetDirectionOffset()).normalized;
        Vector3 direction;

        while (selected.widthMultiplier < laserAttackerOptions.MaxLaserWidth)
        {
            laserOrigin = GetLaserOrigin(originType, barrel, shell);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToPlayer, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToPlayer = direction;
            }

            // Set Material Color
            SetColor(selected, Color.Lerp(laserAttackerOptions.ChargeColor, laserAttackerOptions.ActiveColor, selected.widthMultiplier / laserAttackerOptions.MaxLaserWidth),
                Mathf.Lerp(0, laserAttackerOptions.EmissionIntensityScale, selected.widthMultiplier / laserAttackerOptions.MaxLaserWidth));

            // Change Width
            ChangeWidth(selected, laserAttackerOptions.MaxLaserWidth);

            // Update Laser
            LaserCast(selected.widthMultiplier >= laserAttackerOptions.MaxLaserWidth, selected, direction, laserHasHitDictionary);

            yield return null;
        }

        // Continously change position
        // Active
        for (float t = 0; t < laserAttackerOptions.LaserStayDuration; t += Time.deltaTime)
        {
            laserOrigin = GetLaserOrigin(originType, barrel, shell);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToPlayer, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToPlayer = direction;
            }

            // Set Material Color
            SetColor(selected, laserAttackerOptions.ActiveColor, laserAttackerOptions.EmissionIntensityScale);

            // Update Laser
            LaserCast(true, selected, direction, laserHasHitDictionary);

            // Update Laser Dictionary
            laserHasHitDictionary.Update();

            yield return null;
        }

        // Shrink Laser Width
        // Turning off
        while (selected.widthMultiplier > 0)
        {
            laserOrigin = GetLaserOrigin(originType, barrel, shell);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToPlayer, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToPlayer = direction;
            }

            // Set Material Color
            SetColor(selected, Color.Lerp(laserAttackerOptions.ChargeColor, laserAttackerOptions.ActiveColor, selected.widthMultiplier / laserAttackerOptions.MaxLaserWidth),
                Mathf.Lerp(0, laserAttackerOptions.EmissionIntensityScale, selected.widthMultiplier / laserAttackerOptions.MaxLaserWidth));

            // Change Width
            ChangeWidth(selected, 0);

            // Update Laser
            LaserCast(false, selected, direction, laserHasHitDictionary);

            yield return null;
        }

        selected.enabled = false;
        barrel.IsFiring = false;
    }

    private void ChangeWidth(LineRenderer line, float target)
    {
        line.widthMultiplier = Mathf.MoveTowards(line.widthMultiplier, target, Time.deltaTime * laserAttackerOptions.LaserGrowSpeed);
    }

    private void SetColor(LineRenderer line, Color color, float scale)
    {
        // Regular color
        line.material.color = color;
        line.material.SetColor("_EmissionColor", color * scale);
    }

    private Vector3 GetLaserDirection(LaserAimingType aimingType, BossBarrel barrel, Vector3 origin, Vector3 directionToPlayer, Vector3 randomPoint)
    {
        switch (aimingType)
        {
            case LaserAimingType.FOCUS_PLAYER:
                Vector3 targetDirection = (bossPhaseManager.Player.position - origin).normalized;
                return Vector3.MoveTowards(directionToPlayer, targetDirection, Time.deltaTime * laserAttackerOptions.FollowPlayerSpeed).normalized;
            case LaserAimingType.STRAIGHT:
                return (barrel.transform.position - origin).normalized;
            case LaserAimingType.RANDOM:
                return (randomPoint - origin).normalized;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    private LaserOriginType GetLaserOriginType(BossBarrel line)
    {
        if (line.IsAttached || laserAttackerOptions.KeepAimSourceWhenUnattached)
        {
            return LaserOriginType.SHELL;
        }
        return LaserOriginType.BARREL;
    }

    private LaserAimingType GetLaserAimingType()
    {
        if (laserAttackerOptions.OriginateAtShell)
        {
            return LaserAimingType.STRAIGHT;
        }

        float rand = Random.value;
        // Debug.Log(rand + ", " + laserAttackerOptions.ChanceToTargetPlayer.x / laserAttackerOptions.ChanceToTargetPlayer.y);
        if (laserAttackerOptions.CanTargetPlayer &&
            rand <= laserAttackerOptions.ChanceToTargetPlayer.x / laserAttackerOptions.ChanceToTargetPlayer.y)
        {
            return LaserAimingType.FOCUS_PLAYER;
            // Debug.Log("Aiming to Player");
        }

        // Debug.Log("Aiming randomly");
        return LaserAimingType.RANDOM;
    }

    private void LaserCast(bool doDamage, LineRenderer line, Vector3 direction, TimerDictionary<Collider> hasHit)
    {
        // Create ray
        Ray ray = new Ray(line.transform.position, direction);
        line.SetPosition(0, ray.origin);

        if (doDamage)
        {
            DamageLaserCast(ray, line, hasHit);
        }
        else
        {
            SimpleLaserCast(ray, line);
        }
    }

    private void DamageLaserCast(Ray ray, LineRenderer line, TimerDictionary<Collider> hasHit)
    {
        RaycastHit hit;
        Physics.SphereCast(ray.origin, laserAttackerOptions.LaserRadius, ray.direction, out hit, laserAttackerOptions.LaserRange, laserAttackerOptions.CanHit);
        line.SetPosition(1, ray.GetPoint(laserAttackerOptions.LaserRange));

        if (hit.collider != null)
        {
            // Hit something 
            // a) that has not been hit too recently
            // b) that is damageable
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                // Debug.Log("Hit Damgageable: " + damageable);

                if (hasHit.ContainsKey(hit.collider))
                {
                    // Debug.Log("Dictionary Already Contained: " + hit.collider);
                    return;
                }

                // Damage
                damageable.Damage(laserAttackerOptions.Damage);

                // Debug.Log("Damaged: " + damageable);

                // Add to dictionary so we don't damage again too soon
                hasHit.Add(hit.collider, laserAttackerOptions.TickSpeed);
            }
        }
    }

    private void SimpleLaserCast(Ray ray, LineRenderer line)
    {
        line.SetPosition(1, ray.GetPoint(laserAttackerOptions.LaserRange));
    }

    protected bool GetLasersActive(BossBarrel[] lasers)
    {
        foreach (BossBarrel laser in lasers)
        {
            if (laser.IsFiring) return true;
        }
        return false;
    }

    protected bool GetLasersActive(List<BossBarrel> lasers)
    {
        foreach (BossBarrel laser in lasers)
        {
            if (laser.IsFiring) return true;
        }
        return false;
    }
}
