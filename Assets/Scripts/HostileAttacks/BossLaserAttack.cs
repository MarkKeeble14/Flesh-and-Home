using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossLaserAttack : LaserAttack
{
    [Header("Lasers")]
    private BossLaserAttackOptions bossLaserAttackerOptions;

    protected BossPhaseManager bossPhaseManager;

    protected void Awake()
    {
        // Get reference to boss phase manager
        bossPhaseManager = FindObjectOfType<BossPhaseManager>();

        if (laserAttackerOptions is BossLaserAttackOptions)
        {
            bossLaserAttackerOptions = (BossLaserAttackOptions)laserAttackerOptions;
        }
    }

    protected override Vector3 GetLaserOrigin(LaserBarrel barrel)
    {
        LaserOriginType type;

        if (barrel.IsAttached || bossLaserAttackerOptions.KeepAimSourceWhenUnattached)
        {
            type = LaserOriginType.SHELL;
        }
        else
        {
            type = LaserOriginType.BARREL;

        }

        switch (type)
        {
            case LaserOriginType.BARREL:
                return barrel.transform.position;
            case LaserOriginType.SHELL:
                return bossPhaseManager.ShellEnemyMovement.transform.position;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    protected override LaserAimingType GetLaserAimingType()
    {
        if (bossLaserAttackerOptions.OriginateAtShell)
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
}
