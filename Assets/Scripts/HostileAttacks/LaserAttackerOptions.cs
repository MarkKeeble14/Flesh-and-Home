using UnityEngine;

[CreateAssetMenu(menuName = "LaserAttackerOptions", fileName = "LaserAttackerOptions")]
public class LaserAttackerOptions : ScriptableObject
{
    [SerializeField] private float damage = 2f;
    [SerializeField] private float tickSpeed = .25f;
    [SerializeField] private float laserRange = 1000f;
    [SerializeField] private float laserGrowSpeed = 5f;
    [SerializeField] private float laserStayDuration = 5F;
    [SerializeField] private float maxLaserWidth = 5F;
    [SerializeField] private float laserRadius = .1f;
    [SerializeField] private LayerMask canHit;
    [SerializeField] private Color chargeColor = Color.blue;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private float emissionIntensityScale = 20f;
    [SerializeField] private bool keepAimSourceWhenUnattached;
    [SerializeField] private bool canTargetPlayer;
    [SerializeField] private Vector2 chanceToTargetPlayer;

    public float Damage { get => damage; }
    public float TickSpeed { get => tickSpeed; }
    public float LaserRange { get => laserRange; }
    public float LaserGrowSpeed { get => laserGrowSpeed; }
    public float LaserStayDuration { get => laserStayDuration; }
    public float MaxLaserWidth { get => maxLaserWidth; }
    public float LaserRadius { get => laserRadius; }
    public LayerMask CanHit { get => canHit; }
    public Color ChargeColor { get => chargeColor; }
    public Color ActiveColor { get => activeColor; }
    public float EmissionIntensityScale { get => emissionIntensityScale; }
    public bool KeepAimSourceWhenUnattached { get => keepAimSourceWhenUnattached; }
    public bool CanTargetPlayer { get => canTargetPlayer; }
    public Vector2 ChanceToTargetPlayer { get => chanceToTargetPlayer; }
}
