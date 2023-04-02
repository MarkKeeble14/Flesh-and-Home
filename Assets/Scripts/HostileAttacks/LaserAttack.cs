using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaserAttack : Attack
{
    [Header("Lasers")]
    [SerializeField] protected LaserAttackOptions laserAttackerOptions;
    protected List<LaserBarrel> attackingFrom = new List<LaserBarrel>();

    [SerializeField] private float damageBoost;
    [SerializeField] private float radiusBoost;
    public float Damage { get; set; }
    public float Radius { get; set; }

    private void Awake()
    {
        Damage = laserAttackerOptions.Damage;
        Radius = laserAttackerOptions.LaserRadius;
    }


    public override void Boost()
    {
        Damage += damageBoost;
        Radius += radiusBoost;
        base.Boost();
    }

    public override void Interrupt()
    {
        foreach (LaserBarrel barrel in attackingFrom)
        {
            barrel.IsFiring = false;
        }
        base.Interrupt();
    }

    protected abstract Vector3 GetLaserOrigin(LaserBarrel barrel);

    protected abstract LaserAimingType GetLaserAimingType();

    protected IEnumerator LaserFrom(LaserBarrel barrel, Transform target)
    {
        if (barrel.IsFiring)
        {
            // Debug.Log("Already Firing");
            yield break;
        }

        if (barrel.Disabled)
        {
            // Debug.Log("Is Disabled");
            yield break;
        }

        barrel.IsFiring = true;

        attackingFrom.Add(barrel);

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
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 randomFocusPoint = randomDirection * laserAttackerOptions.LaserRange;
        Vector3 laserOrigin = GetLaserOrigin(barrel);
        Vector3 directionToTarget = (target.position - laserOrigin + laserAttackerOptions.GetDirectionOffset()).normalized;
        Vector3 direction;

        while (selected.widthMultiplier < laserAttackerOptions.MaxLaserWidth)
        {
            laserOrigin = GetLaserOrigin(barrel);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToTarget, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToTarget = direction;
            }

            // Set Material Color
            SetColor(selected, laserAttackerOptions.LaserVisuals.GetDefaultColor());

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
            laserOrigin = GetLaserOrigin(barrel);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToTarget, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToTarget = direction;
            }

            // Set Material Color
            SetColor(selected, laserAttackerOptions.LaserVisuals.GetMaxColor());

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
            laserOrigin = GetLaserOrigin(barrel);
            direction = GetLaserDirection(aimingType, barrel, laserOrigin, directionToTarget, randomFocusPoint);
            if (aimingType == LaserAimingType.FOCUS_PLAYER)
            {
                directionToTarget = direction;
            }

            // Set Material Color
            SetColor(selected, laserAttackerOptions.LaserVisuals.GetLerpedColor(selected.widthMultiplier / laserAttackerOptions.MaxLaserWidth));

            // Change Width
            ChangeWidth(selected, 0);

            // Update Laser
            LaserCast(false, selected, direction, laserHasHitDictionary);

            yield return null;
        }

        attackingFrom.Remove(barrel);
        selected.enabled = false;
        barrel.IsFiring = false;
    }

    private void ChangeWidth(LineRenderer line, float target)
    {
        line.widthMultiplier = Mathf.MoveTowards(line.widthMultiplier, target, Time.deltaTime * laserAttackerOptions.LaserGrowSpeed);
    }

    private void SetColor(LineRenderer line, Color color)
    {
        // Regular color
        line.material.color = color;
        line.material.SetColor("_EmissionColor", laserAttackerOptions.LaserVisuals.GetEmmissiveColor(color));
    }

    private Vector3 GetLaserDirection(LaserAimingType aimingType, LaserBarrel barrel, Vector3 origin, Vector3 directionToPlayer, Vector3 randomPoint)
    {
        switch (aimingType)
        {
            case LaserAimingType.FOCUS_PLAYER:
                Vector3 targetDirection = (GameManager._Instance.PlayerAimAt.position - origin).normalized;
                return Vector3.MoveTowards(directionToPlayer, targetDirection, Time.deltaTime * laserAttackerOptions.FollowPlayerSpeed).normalized;
            case LaserAimingType.STRAIGHT:
                return (barrel.transform.position - origin).normalized;
            case LaserAimingType.RANDOM:
                return (randomPoint - origin).normalized;
            default:
                throw new UnhandledSwitchCaseException();
        }
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
        Physics.SphereCast(ray.origin + ray.direction, Radius, ray.direction, out hit, laserAttackerOptions.LaserRange, laserAttackerOptions.CanHit);
        line.SetPosition(1, hit.collider == null ? ray.GetPoint(laserAttackerOptions.LaserRange) : hit.point);

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
                damageable.Damage(Damage, DamageSource.ENEMY_LASER);

                // Debug.Log("Damaged: " + damageable);

                // Add to dictionary so we don't damage again too soon
                hasHit.Add(hit.collider, laserAttackerOptions.TickSpeed);
            }
        }
    }

    private void SimpleLaserCast(Ray ray, LineRenderer line)
    {
        RaycastHit hit;
        // + (ray.direction) is to ensure attack doesn't hit attacking object
        Physics.SphereCast(ray.origin + ray.direction, laserAttackerOptions.LaserRadius, ray.direction, out hit, laserAttackerOptions.LaserRange, laserAttackerOptions.CanHit);
        line.SetPosition(1, hit.collider == null ? ray.GetPoint(laserAttackerOptions.LaserRange) : hit.point);
    }

    protected bool GetLasersActive(LaserBarrel[] lasers)
    {
        foreach (LaserBarrel laser in lasers)
        {
            if (laser.IsFiring) return true;
        }
        return false;
    }

    protected bool GetLasersActive(List<LaserBarrel> lasers)
    {
        foreach (LaserBarrel laser in lasers)
        {
            if (laser.IsFiring) return true;
        }
        return false;
    }
}
