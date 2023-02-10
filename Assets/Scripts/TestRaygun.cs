using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class TestRaygun : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float overheatAfter;
    [SerializeField] private float overheatDamage;
    [SerializeField] private float maxDistance;
    [SerializeField] private Transform shootFrom;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float impactForce = 80f;
    
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
    
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Material explosion;
    [SerializeField] private Material lazerMat;
    [SerializeField] private ParticleSystem smokeMat;
    [SerializeField] private GameObject lazerPrefab;

    private Light li;
    private InputManager inputManager;

    private float trackOverheatTimer;
    private bool overheated;
    private bool active;

    private void Start()
    {
        li = lazerPrefab.GetComponent<Light>();
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
                trackOverheatTimer -= Time.deltaTime;
            }
        }

        if (trackOverheatTimer > (overheatAfter / 2.0f))
        {
            li.color = Color.Lerp(defaultColor, overheatColor, 1.5f);
        } 
        else if (trackOverheatTimer > 0 && trackOverheatTimer < (overheatAfter / 2.0f))
        {
            li.color = Color.Lerp(overheatColor, defaultColor, 1.5f);
        }
        
        // Audio
        laserConstantSound.enabled = active;
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

        // Tell Laser Cutter to play
        laserStartSound.PlayOneShot(source);

        // While the player has the button pressed
        while (inputManager.PlayerInputActions.Player.Shoot.IsPressed())
        {
            trackOverheatTimer += Time.deltaTime;
            
            // Interpolate Color based on how close we are to overheating
            Color c = GetBeamColor(defaultColor, overheatColor, trackOverheatTimer / overheatAfter);

            //Change lazer color
            lazerMat.color = c;
            lazerMat.SetColor("_EmissionColor", c * emissionIntensity);
            
            //Change explosion flash color *need to fix*
            explosion.color = c;
            explosion.SetColor("_EmissionColor", c * 3.0f);

            smokeMat.startColor = c;

            muzzleFlash.startColor = c;
            muzzleFlash.Play();

            // Break if overheating
            if (trackOverheatTimer > overheatAfter)
            {
                overheated = true;
                break;
            }
            
            GameObject laser = Instantiate(shotPrefab, muzzleTransform.position, muzzleTransform.rotation);
            
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, maxDistance))
            {
                EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    target.takeDamage(damage);
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                
                laser.GetComponent<ShotBehavior>().setTarget(hit.point);
                
                Destroy(impact, 1f);
            }
            Destroy(laser, 0.5f);
            yield return null;
        }

        // Stop playing due to overheat
        if (overheated)
        {
            StartCoroutine(HasOverheated());
        }

        // Signify ended
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
