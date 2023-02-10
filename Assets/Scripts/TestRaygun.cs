using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class TestRaygun : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private ShotBehavior shotPrefab;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float impactForce = 80f;
    // Fire rate is measured in shots per second
    [SerializeField] private float fireRate = 5f;
    private float fireRateTimer;
    [SerializeField] private LayerMask canHit;
    public LayerMask CanHit => canHit;

    [Header("Muzzle Flash")]
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Particles")]
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private Material lazerMat;

    [Header("Settings")]
    [SerializeField] private float overheatAfter = 50f;
    [SerializeField] private float overheatDamage = 3f;
    [SerializeField] private float maxDistance = 500f;
    [SerializeField] private float heatPerShot = 1f;
    [SerializeField] private float heatDissapationRate;
    private float trackOverheatTimer;
    private bool overheated;
    private bool active;

    [Header("Visuals")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color overheatColor;
    [SerializeField] private float emissionIntensity;
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
                trackOverheatTimer -= Time.deltaTime * heatDissapationRate;
            }
        }

        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        /*
        if (trackOverheatTimer > (overheatAfter / 2.0f))
        {
            light.color = Color.Lerp(defaultColor, overheatColor, 1.5f);
        }
        else if (trackOverheatTimer > 0 && trackOverheatTimer < (overheatAfter / 2.0f))
        {
            light.color = Color.Lerp(overheatColor, defaultColor, 1.5f);
        }
        */

        // Audio
        overheatConstantSound.enabled = overheated;

        // Update UI
        showOverheatBar.Set(trackOverheatTimer, overheatAfter);
        showOverheatBar.SetColor(overheated ? overheatColor : defaultColor);
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
            trackOverheatTimer += heatPerShot;

            // Break if overheating
            if (trackOverheatTimer > overheatAfter)
            {
                overheated = true;
                break;
            }

            // Cooldown
            // Fire rate is measured in shots per second
            fireRateTimer = 1 / fireRate;

            // Audio
            shootSound.PlayOneShot(source);

            // Crosshair
            CrosshairController._Instance.Spread(2);

            // Interpolate Color based on how close we are to overheating
            currentColor = GetBeamColor(defaultColor, overheatColor, trackOverheatTimer / overheatAfter);

            // Change lazer color
            lazerMat.color = currentColor;
            lazerMat.SetColor("_EmissionColor", currentColor * emissionIntensity);

            // Change Muzzle flash color
            muzzleFlash.startColor = currentColor;

            // Play Muzzle Flash
            muzzleFlash.Play();

            // Spawn Projectile
            ShotBehavior laser = Instantiate(shotPrefab, muzzleTransform.position, muzzleTransform.rotation);
            bool hasHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, canHit);

            // if we've hit something, spawn particles and try to do damage
            if (hasHit)
            {
                laser.setTarget(hit.point, this);
                // Spawn Particles
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

                // Try to do damage
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(damage, -hit.normal * impactForce);
                }
            }
            else
            {
                laser.setTarget(Camera.main.transform.forward * maxDistance, this);
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
        playerDamageable.Damage(overheatDamage);

        // Wait until Overheat has cooled down
        yield return new WaitUntil(() => trackOverheatTimer <= 0);

        overheatEndSound.PlayOneShot(source);

        // Done with Overheat
        overheated = false;
    }
}
