using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Rifle : MonoBehaviour
{
    [System.Serializable]
    public struct RifleSettings
    {
        public ShotBehavior projectile;
        public float impactForce;
        public float crosshairSpread;

        // Laser info
        public float speed;
        public float damage;
        public float shotsPerSecond;
        public float maxDistance;
        public LayerMask canHit;
        public LayerMask canDamage;

        // Overheat info
        public OverheatSettings overheatSettings;

        // Visual info
        public LaserVisuals visuals;
    }

    [SerializeField] private RifleSettings rifleSettings;

    private float fireRateTimer;
    public LayerMask CanHit => rifleSettings.canHit;

    [Header("Muzzle Flash")]
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Particles")]
    [SerializeField] private Material lazerMat;

    [Header("Settings")]
    private float trackOverheatTimer;
    private bool overheated;
    private bool active;

    [Header("Visuals")]
    private Color currentColor;
    public Color CurrentColor => currentColor;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer shootSound;
    [SerializeField] private AudioSource overheatConstantSound;
    [SerializeField] private AudioClipContainer overheatStartSound;
    [SerializeField] private AudioClipContainer overheatEndSound;
    [SerializeField] private AudioSource source;

    [Header("References")]
    [SerializeField] private ImageSliderBar showOverheatBar;

    private InputManager inputManager;
    private IDamageable playerDamageable;

    private void Awake()
    {
        // Get reference
        playerDamageable = GetComponent<IDamageable>();
    }

    private void Start()
    {
        // Get reference to InputManager
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Shoot.started += Shoot;
    }

    private void OnDestroy()
    {
        // Remove input action
        inputManager.PlayerInputActions.Player.Shoot.started -= Shoot;
    }

    private void Update()
    {
        // li.color -= (Color.white / 2.0f) * Time.deltaTime;
        // Reduce timer if not shooting
        // Later should replace this time based calculation with a more accurate physics based on (heat dissapation iirc)
        if (!active)
        {
            if (trackOverheatTimer < 0)
            {
                trackOverheatTimer = 0;
            }
            else
            {
                trackOverheatTimer -= Time.deltaTime * rifleSettings.overheatSettings.heatDissapationRate;
            }
        }

        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        // Audio
        overheatConstantSound.enabled = overheated;

        // Update UI
        showOverheatBar.Set(trackOverheatTimer, rifleSettings.overheatSettings.overheatAfter);
        showOverheatBar.SetColor(overheated ? rifleSettings.visuals.GetMaxColor() :
            rifleSettings.visuals.GetLerpedColor(trackOverheatTimer / rifleSettings.overheatSettings.overheatAfter));
    }

    private void Shoot(InputAction.CallbackContext ctx)
    {
        if (overheated) return;
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        RaycastHit hit;

        // Started firing
        active = true;

        // While the player has the button pressed
        while (inputManager.PlayerInputActions.Player.Shoot.IsPressed())
        {
            yield return new WaitUntil(() => fireRateTimer <= 0);

            // We are firing and thus generating heat
            trackOverheatTimer += rifleSettings.overheatSettings.heatAccrualRate;

            // Break if overheating
            if (trackOverheatTimer > rifleSettings.overheatSettings.overheatAfter)
            {
                overheated = true;
                break;
            }

            // Cooldown
            // Fire rate is measured in shots per second
            fireRateTimer = 1 / rifleSettings.shotsPerSecond;

            // Audio
            shootSound.PlayOneShot(source);

            // Crosshair
            CrosshairManager._Instance.Spread(CrosshairType.RIFLE, rifleSettings.crosshairSpread);

            // Interpolate Color based on how close we are to overheating
            currentColor = rifleSettings.visuals.GetLerpedColor(trackOverheatTimer / rifleSettings.overheatSettings.overheatAfter);

            // Change lazer color
            lazerMat.color = currentColor;
            lazerMat.SetColor("_EmissionColor", rifleSettings.visuals.GetEmmissiveColor(currentColor));

            // Change Muzzle flash color
            muzzleFlash.startColor = currentColor;

            // Play Muzzle Flash
            muzzleFlash.Play();

            // Spawn Projectile
            ShotBehavior laser = Instantiate(rifleSettings.projectile, muzzleTransform.position, muzzleTransform.rotation);
            Ray shootRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red, 10f);
            bool hasHit = Physics.Raycast(shootRay, out hit, rifleSettings.maxDistance, rifleSettings.canHit);

            // if we've hit something, spawn particles and try to do damage
            if (hasHit)
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.blue, 10f);
                laser.SetTarget(hit.point, CurrentColor, rifleSettings.canHit, rifleSettings.canDamage, rifleSettings.speed, rifleSettings.damage, rifleSettings.impactForce);
            }
            else
            {
                // So this actually doesn't work very well, luckily we should almost always be hitting something with raycast wether it be environment or enemy
                Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * rifleSettings.maxDistance, Color.green, 10f);
                laser.SetTarget(Camera.main.transform.forward * rifleSettings.maxDistance, CurrentColor, rifleSettings.canHit, rifleSettings.canDamage, rifleSettings.speed, rifleSettings.damage, rifleSettings.impactForce);
            }

            yield return null;
        }

        // Stop playing due to overheat
        if (overheated)
        {
            StartCoroutine(HasOverheated());
        }

        // Signify ended
        active = false;
    }

    private Color GetBeamColor(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, t);
    }

    private IEnumerator HasOverheated()
    {
        overheatStartSound.PlayOneShot(source);

        // Deal damage to play if they overheat
        playerDamageable.Damage(rifleSettings.overheatSettings.overheatDamage);

        // Wait until Overheat has cooled down
        yield return new WaitUntil(() => trackOverheatTimer <= 0);

        overheatEndSound.PlayOneShot(source);

        // Done with Overheat
        overheated = false;
    }
}
