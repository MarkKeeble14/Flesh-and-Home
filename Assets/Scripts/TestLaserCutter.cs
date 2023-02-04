using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class TestLaserCutter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float overheatAfter;
    [SerializeField] private float overheatDamage;
    [SerializeField] private float maxDistance;
    [SerializeField] private Transform shootFrom;

    [Header("Visuals")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color overheatColor;
    [SerializeField] private float emissionIntensity;

    [SerializeField] private float fullWidth;
    [SerializeField] private float beamGrowSpeed;
    [SerializeField] private float beamCollapseSpeed;
    [SerializeField] private float beamOverheatCollapseSpeed;

    [Header("References")]
    [SerializeField] private AudioSource laserConstantSound;
    [SerializeField] private AudioClipContainer laserStartSound;
    [SerializeField] private AudioClipContainer laserEndSound;
    [SerializeField] private AudioSource overheatConstantSound;
    [SerializeField] private AudioClipContainer overheatStartSound;
    [SerializeField] private AudioClipContainer overheatEndSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private ImageSliderBar showOverheatBar;
    private InputManager inputManager;
    private LineRenderer lineRenderer;
    private Material material;

    private float trackOverheatTimer;
    private bool overheated;
    private bool active;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        material = lineRenderer.material;
    }

    private void Start()
    {
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.FireAttachment.started += Fire;
    }

    private void Update()
    {
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
                trackOverheatTimer -= Time.deltaTime;
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
        showOverheatBar.Set(trackOverheatTimer, overheatAfter);
        showOverheatBar.SetColor(overheated ? overheatColor : defaultColor);
    }

    private void Fire(InputAction.CallbackContext ctx)
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

        // While the player has the button pressed
        while (inputManager.PlayerInputActions.Player.FireAttachment.IsPressed())
        {
            trackOverheatTimer += Time.deltaTime;

            // Interpolate Color based on how close we are to overheating
            Color c = GetBeamColor(defaultColor, overheatColor, trackOverheatTimer / overheatAfter);

            material.color = c;
            material.SetColor("_EmissionColor", c * emissionIntensity);

            // Break if overheating
            if (trackOverheatTimer > overheatAfter)
            {
                overheated = true;
                break;
            }

            // Determine position and direction
            ray = new Ray(shootFrom.position, Camera.main.transform.forward);
            Physics.Raycast(ray, out hit, maxDistance);
            if (hit.transform != null)
            {
                lineRenderer.SetPosition(0, shootFrom.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(0, shootFrom.position);
                lineRenderer.SetPosition(1, ray.GetPoint(maxDistance));
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

    private Color GetBeamColor(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, t);
    }

    private IEnumerator HasOverheated()
    {
        overheatStartSound.PlayOneShot(source);

        // Wait until Overheat has cooled down
        yield return new WaitUntil(() => trackOverheatTimer <= 0);

        overheatEndSound.PlayOneShot(source);

        // Done with Overheat
        overheated = false;
    }
}
