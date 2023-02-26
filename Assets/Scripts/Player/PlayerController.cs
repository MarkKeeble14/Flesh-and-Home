using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    Vector3 movementVector;
    Vector3 jetpackAirDashVelocity;
    private bool hasLanded;
    private bool flying;
    private float jetpackTimer;

    [Header("Player Controller")]

    #region Movement

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float airStrafeSpeed = .5f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float mass = 3.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Vector3 playerVelocity;


    [Header("Audio")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioSource footstepsSource;

    #endregion

    [Header("Bobbing")]
    [SerializeField] private float bobIdleSpeed = 0.025f;
    [SerializeField] private float bobMovingSpeed = 0.05f;
    [SerializeField] private float bobSprintingSpeed = 0.075f;
    [SerializeField] private float bobIdleStrength = 2f;
    [SerializeField] private float bobMovingStrength = 6f;
    [SerializeField] private float bobSprintingStrength = 10f;

    [SerializeField] private Transform lazerParent;
    private Vector3 lazerParentOrigin;
    private Vector3 targetWeaponBobPosition;
    private float idleCounter;
    private float movementCounter;
    private float sprintingCounter;

    #region Jetpack

    [Header("Jetpack")]
    // Settings
    [Header("Settings")]
    private bool hasJetpack;
    [SerializeField] private float jetpackForce = 25.0f;
    [SerializeField] private float maxJetpackDuration = 3f;
    [SerializeField] private float jetpackDashPower = 10f;
    [SerializeField] private float dashVelocityFalloffSpeedFlying = 1f;
    [SerializeField] private float dashVelocityFalloffSpeedGrounded;
    [SerializeField] private float jetpackDashCooldown = 5f;
    private float jetpackDashCooldownTimer;
    [SerializeField] private Color jetpackDashAvailableColor;
    [SerializeField] private Color jetpackDashUnavailableColor;
    [SerializeField] private AudioClipContainer jetpackDashClip;
    private bool CanUseDash
    {
        get
        {
            return jetpackDashCooldownTimer <= 0;
        }
    }

    // Fuel
    [SerializeField] private FuelStore fuelStore;
    [SerializeField] private float fuelConsumptionRate = 1f;
    [SerializeField] private ParticleSystem jetpackParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource jetpackAudioSource;
    [SerializeField] private AudioClip jetpackStartClip;
    [SerializeField] private AudioClip jetpackEndClip;

    [Header("References")]
    [SerializeField] private ImageSliderBar jetpackDisplay;
    [SerializeField] private ImageSliderBar jetpackDashCooldownBar;
    [SerializeField] private ParticleSystem dashParticleSystem;
    #endregion

    private float MoveSpeed
    {
        get
        {
            return !isGrounded ? airStrafeSpeed : (baseSpeed * (IsSprinting ? sprintSpeed : 1));
        }
    }

    private bool IsSprinting
    {
        get
        {
            return inputManager.PlayerInputActions.Player.Sprint.IsPressed();
        }
    }

    #region Collisions
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Transform bonkCheckPosition;
    private bool isGrounded;
    [SerializeField] private float environmentCollisionCheckRadius = 0.1f;
    [SerializeField] private LayerMask environmentCollisions;
    private bool isBonking;

    #endregion

    private CharacterController controller;
    private InputManager inputManager;
    private Transform cameraTransform;

    [SerializeField] private AudioSource source;

    private void Awake()
    {
        // Set Fuel
        fuelStore.Reset();

        // Update UI
        jetpackDisplay.Set(0, maxJetpackDuration);
    }

    private void Start()
    {

        // Get References
        lazerParentOrigin = lazerParent.localPosition;

        controller = GetComponent<CharacterController>();

        cameraTransform = Camera.main.transform;

        // Fetch the Input Manager Singleton
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Jump.performed += Jump;
        inputManager.PlayerInputActions.Player.Sprint.performed += TryDash;
    }

    private void OnDestroy()
    {
        // Remove input actions
        inputManager.PlayerInputActions.Player.Jump.performed -= Jump;
        inputManager.PlayerInputActions.Player.Sprint.performed -= TryDash;
    }

    void Update()
    {
        // Set collision states
        isGrounded = Physics.CheckSphere(groundCheckPosition.position, environmentCollisionCheckRadius, environmentCollisions);
        isBonking = Physics.CheckSphere(bonkCheckPosition.position, environmentCollisionCheckRadius, environmentCollisions);

        if (isGrounded && playerVelocity.y < 0)
        {
            if (!hasLanded)
            {
                hasLanded = true;
                source.PlayOneShot(landClip);
                jetpackTimer = 0;
            }
            playerVelocity.y = 0f;
        }

        // Control Dash
        // if velocity is not 0, move towards to 0
        if (jetpackAirDashVelocity != Vector3.zero)
        {
            // if grounded, will fall off much quicker
            jetpackAirDashVelocity = Vector3.Lerp(jetpackAirDashVelocity, Vector3.zero,
                (isGrounded ? dashVelocityFalloffSpeedGrounded : dashVelocityFalloffSpeedFlying) * Time.deltaTime);
        }

        // Set UI for Air Dash Cooldown
        jetpackDashCooldownBar.Set(jetpackDashCooldownTimer, jetpackDashCooldown);
        jetpackDashCooldownBar.SetColor(CanUseDash ? jetpackDashAvailableColor : jetpackDashUnavailableColor);

        // Movement
        Vector2 movement = inputManager.GetPlayerMovement();
        movementVector = new Vector3(movement.x, 0f, movement.y);
        movementVector = cameraTransform.forward * movementVector.z + cameraTransform.right * movementVector.x;
        movementVector.y = 0f;
        controller.Move((movementVector + jetpackAirDashVelocity) * MoveSpeed * Time.deltaTime);

        // Enable footsteps audio source when walking
        footstepsSource.enabled = isGrounded && movementVector != Vector3.zero;

        // Changes the height position of the player..
        if (inputManager.PlayerJumpedThisFrame() && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -mass * gravityValue * Time.deltaTime);
        }

        // Only subtract velocity if not grounded and not hitting a ceiling while jetpacking
        if (!isGrounded)
        {
            if (!(flying && isBonking))
            {
                playerVelocity.y += gravityValue * Time.deltaTime;
            }
        }

        // Subtract from air dash timer
        if (jetpackDashCooldownTimer > 0)
        {
            jetpackDashCooldownTimer -= Time.deltaTime;
        }

        // Debug.Log("Velocity: " + playerVelocity);

        // Move player
        controller.Move(playerVelocity * Time.deltaTime);

        // Headbobbing
        ExecuteHeadbob();
    }

    private void ExecuteHeadbob()
    {
        // Headbobbing
        if (isGrounded && IsSprinting)
        {
            // sprinting
            HeadBob(sprintingCounter, 0, bobSprintingStrength);
            sprintingCounter += Time.deltaTime * bobSprintingSpeed;
            lazerParent.localPosition =
                Vector3.Lerp(lazerParent.localPosition, targetWeaponBobPosition, Time.deltaTime);
        }
        else if (!IsSprinting && isGrounded && movementVector != Vector3.zero)
        {
            // moving
            HeadBob(movementCounter, 0, bobMovingStrength);
            movementCounter += Time.deltaTime * bobMovingSpeed;
            lazerParent.localPosition =
                Vector3.Lerp(lazerParent.localPosition, targetWeaponBobPosition, Time.deltaTime);
        }
        else if (isGrounded && movementVector == Vector3.zero)
        {
            // idle
            HeadBob(idleCounter, 0, bobIdleStrength);
            idleCounter += Time.deltaTime * bobIdleSpeed;
            lazerParent.localPosition =
                Vector3.Lerp(lazerParent.localPosition, targetWeaponBobPosition, Time.deltaTime);
        }
    }

    private void HeadBob(float z, float x_intensity, float y_intensity)
    {
        targetWeaponBobPosition = lazerParentOrigin + new Vector3(Mathf.Cos(z) * x_intensity, Mathf.Sin(z * 2) * y_intensity,
            0);
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            // Debug.Log("Jump");
            hasLanded = false;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -mass * gravityValue);

            source.PlayOneShot(jumpClip);
        }
        else if (hasJetpack && !flying)
        {
            hasLanded = false;
            StartCoroutine(Jetpack());
        }
    }

    [ContextMenu("AquireJetpack")]
    public void AquireJetpack()
    {
        hasJetpack = true;
    }

    private IEnumerator Jetpack()
    {
        // Debug.Log("Start Jetpack");

        flying = true;

        // Make noise
        source.PlayOneShot(jetpackStartClip);
        jetpackAudioSource.enabled = true;
        jetpackParticles.Play();

        // Player must be holding the button, have time left, and also must have fuel left to burn
        while (inputManager.PlayerInputActions.Player.Jump.IsPressed() && jetpackTimer < maxJetpackDuration && fuelStore.CurrentFuel > 0 && !isGrounded)
        {
            // Update timer
            jetpackTimer += Time.deltaTime;

            // Update UI
            jetpackDisplay.Set(jetpackTimer, maxJetpackDuration);

            // Use Fuel
            fuelStore.AlterFuel(-Time.deltaTime * fuelConsumptionRate);

            // Add force to move, but only if there is space to move upwards
            if (isBonking)
            {
                playerVelocity.y = 0;
            }
            else
            {
                playerVelocity.y += Mathf.Sqrt(jetpackForce * -3.0f * gravityValue * Time.deltaTime);
            }

            yield return null;
        }

        // Make noise
        source.PlayOneShot(jetpackEndClip);
        jetpackAudioSource.enabled = false;
        jetpackParticles.Stop();

        // Update UI
        jetpackDisplay.Set(0, maxJetpackDuration);

        flying = false;
        // Debug.Log("End Jetpack");
    }

    private void TryDash(InputAction.CallbackContext ctx)
    {
        if (!CanUseDash)
        {
            if (jetpackDashCooldownTimer > 0)
            {
                // Debug.Log("Air Dash On Cooldown; Can't use Air Dash");
                return;
            }
        }

        // Debug.Log("Activate Air Dash");

        // Audio
        jetpackDashClip.PlayOneShot(source);

        // Add Force
        Vector3 dashForce = movementVector * jetpackDashPower;
        jetpackAirDashVelocity += dashForce;

        // Spawn Particles
        Instantiate(dashParticleSystem, transform.position, Quaternion.LookRotation(-dashForce)).Play();

        // Set cooldown
        jetpackDashCooldownTimer = jetpackDashCooldown;
    }
}
