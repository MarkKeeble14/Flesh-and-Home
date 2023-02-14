using UnityEngine;

[CreateAssetMenu(menuName = "LaserAttackOptions/Base", fileName = "LaserAttackOptions")]
public class LaserAttackOptions : ScriptableObject
{
    [Header("Basic")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float tickSpeed = .25f;
    [SerializeField] private float laserRange = 1000f;
    [SerializeField] private LayerMask canHit;
    [SerializeField] private float laserRadius = .1f;

    [Header("Beam")]
    [SerializeField] private BeamSettings beamSettings;
    [SerializeField] private float laserStayDuration = 5f;

    [Header("Visuals")]
    [SerializeField] private LaserVisuals visuals;

    [Header("Targeting")]
    [SerializeField] private bool canTargetPlayer;
    [SerializeField] private float followPlayerSpeed;
    [SerializeField] private Vector2 chanceToTargetPlayer;
    [SerializeField] private Vector2 minMaxLaserOffset = new Vector2(2.5f, 5f);


    public float Damage { get => damage; }
    public float TickSpeed { get => tickSpeed; }
    public float LaserRange { get => laserRange; }
    public float LaserGrowSpeed { get => beamSettings.beamGrowSpeed; }
    public float LaserStayDuration { get => laserStayDuration; }
    public float MaxLaserWidth { get => beamSettings.fullWidth; }
    public float LaserRadius { get => laserRadius; }
    public LayerMask CanHit { get => canHit; }
    public LaserVisuals LaserVisuals { get => visuals; }
    public bool CanTargetPlayer { get => canTargetPlayer; }
    public Vector2 ChanceToTargetPlayer { get => chanceToTargetPlayer; }
    public float FollowPlayerSpeed { get => followPlayerSpeed; }
    public Vector2 MinMaxLaserOffset { get => minMaxLaserOffset; }

    public Vector3 GetDirectionOffset()
    {
        return new Vector3(
            Random.Range(MinMaxLaserOffset.x, MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1),
            Random.Range(MinMaxLaserOffset.x, MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1),
            Random.Range(MinMaxLaserOffset.x, MinMaxLaserOffset.y) * (Random.value >= .5f ? 1 : -1)
            );
    }
}
