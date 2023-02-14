using UnityEngine;

[CreateAssetMenu(menuName = "LaserAttackOptions/Boss", fileName = "BossLaserAttackOptions")]
public class BossLaserAttackOptions : LaserAttackOptions
{
    [SerializeField] private bool keepAimSourceWhenUnattached;
    [SerializeField] private bool originateAtShell;

    public bool KeepAimSourceWhenUnattached { get => keepAimSourceWhenUnattached; }
    public bool OriginateAtShell { get => originateAtShell; }
}
