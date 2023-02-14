using UnityEngine;

public abstract class StandardLaserAttack : LaserAttack
{
    protected override Vector3 GetLaserOrigin(LaserBarrel barrel)
    {
        return barrel.transform.position;
    }

    protected override LaserAimingType GetLaserAimingType()
    {
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
