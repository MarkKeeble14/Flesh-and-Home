using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class LaserCutterSettings : WeaponAttachmentController
{
    [System.Serializable]
    public struct LaserSettings
    {
        public float damage;
        public float tickRate;
        public float overheatDamage;
        public float overheatAfter;
        public float maxDistance;
        public LayerMask canHit;
        public float heatDissapationRate;
    }

    [SerializeField] private LaserSettings laserSettings;
    private TimerDictionary<Collider> hitTickBetweenTimer = new TimerDictionary<Collider>();

    [Header("Visuals")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color overheatColor;
    [SerializeField] private float emissionIntensity;

    [SerializeField] private float fullWidth;
    [SerializeField] private float beamGrowSpeed;
    [SerializeField] private float beamCollapseSpeed;
    [SerializeField] private float beamOverheatCollapseSpeed;

    [Header("References")]
    [SerializeField] private Transform shootFrom;
    [SerializeField] private AudioSource laserConstantSound;
    [SerializeField] private AudioClipContainer laserStartSound;
    [SerializeField] private AudioClipContainer laserEndSound;
    [SerializeField] private AudioSource overheatConstantSound;
    [SerializeField] private AudioClipContainer overheatStartSound;
    [SerializeField] private AudioClipContainer overheatEndSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private ImageSliderBar showOverheatBar;
    private IDamageable playerDamageable;
    private LineRenderer lineRenderer;
    private Material material;

    private float trackOverheatTimer;
    private bool overheated;
    private bool active;

    private void Awake()
    {
        playerDamageable = GetComponent<IDamageable>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        material = lineRenderer.material;
    }

    public void Update()
    {
        // Update Dictionary
        hitTickBetweenTimer.Update();

        // Reduce timer if not shooting
        // Later should replace this time based calculation with a more accurate physics based on (heat dissapation iirc)
        if (!active)
        {
            if (trackOverheatTimer > 0)
            {
                trackOverheatTimer -= Time.deltaTime * laserSettings.heatDissapationRate;
            }
            else
            {
                trackOverheatTimer = 0;
            }

            // Shrink Beam
            if (lineRenderer.widthMultiplier > 0)
            {
                lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, 0, (overheated ? beamOverheatCollapseSpeed : beamCollapseSpeed) * Time.deltaTime);
            }
        }
        else
        {
            // Grow Beam
            if (lineRenderer.widthMultiplier < fullWidth)
            {
                lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, fullWidth, beamGrowSpeed * Time.deltaTime);
            }
        }

        // Show Line Renderer only if active
        lineRenderer.enabled = (active || lineRenderer.widthMultiplier > 0) || (overheated && lineRenderer.widthMultiplier > 0);

        // Audio
        laserConstantSound.enabled = active;
        overheatConstantSound.enabled = overheated;

        // Update UI
        showOverheatBar.Set(trackOverheatTimer, laserSettings.overheatAfter);
        showOverheatBar.SetColor(overheated ? overheatColor : defaultColor);
    }

    public override void Fire(InputAction.CallbackContext ctx)
    {
        if (overheated) return;
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        RaycastHit hit;
        Ray ray;

        // Reset width multiplier
        lineRenderer.widthMultiplier = 0;

        // Started firing
        active = true;

        // Tell Laser Cutter to play
        laserStartSound.PlayOneShot(source);

        Collider rayHitting;
        // While the player has the button pressed
        while (InputManager._Instance.PlayerInputActions.Player.FireAttachment.IsPressed())
        {
            trackOverheatTimer += Time.deltaTime;

            // Interpolate Color based on how close we are to overheating
            Color c = Color.Lerp(defaultColor, overheatColor, trackOverheatTimer / laserSettings.overheatAfter);
            material.color = c;
            material.SetColor("_EmissionColor", c * emissionIntensity);

            // Break if overheating
            if (trackOverheatTimer > laserSettings.overheatAfter)
            {
                overheated = true;
                break;
            }

            // Move Crosshair
            CrosshairController._Instance.Spread(.25f);

            // Determine position and direction
            ray = new Ray(shootFrom.position, Camera.main.transform.forward);
            Physics.Raycast(ray, out hit, laserSettings.maxDistance, laserSettings.canHit);

            // Set Position
            lineRenderer.SetPosition(0, ray.origin);

            // Try to hit object
            if (hit.transform != null)
            {
                // We've hit something
                rayHitting = hit.collider;

                // Set Renderer to stop at where we hit
                lineRenderer.SetPosition(1, hit.point);

                // Debug.Log("Ray Hitting: " + rayHitting);

                // if other object has not been hit too recently, and posseses a component which implements IDamageable, use it to deal damage
                if (!hitTickBetweenTimer.ContainsKey(rayHitting)
                    && rayHitting.TryGetComponent(out IDamageable damageable))
                {
                    // Debug.Log("Has Damageable Component: " + damageable + "; Dealing Damage to: " + rayHitting);

                    // Deal damage
                    damageable.Damage(laserSettings.damage);

                    // Add to dictionary so we know not to hit again too quickly
                    hitTickBetweenTimer.Add(rayHitting, laserSettings.tickRate);
                }
            }
            else
            {
                // Hit nothing
                // Set Renderer to some point along ray
                lineRenderer.SetPosition(1, ray.GetPoint(laserSettings.maxDistance));
            }

            yield return null;
        }

        // Stop playing due to overheat
        if (overheated)
        {
            StartCoroutine(HasOverheated());
        }

        // Signifiy ended
        laserEndSound.PlayOneShot(source);

        active = false;
    }

    private IEnumerator HasOverheated()
    {
        overheatStartSound.PlayOneShot(source);

        // Deal damage to play if they overheat
        playerDamageable.Damage(laserSettings.overheatDamage);

        // Wait until Overheat has cooled down
        yield return new WaitUntil(() => trackOverheatTimer <= 0);

        overheatEndSound.PlayOneShot(source);

        // Done with Overheat
        overheated = false;
    }
}
