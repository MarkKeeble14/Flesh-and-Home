using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : KillableEntity
{
    [SerializeField] private ImageSliderBar playerHpBar;
    [SerializeField] private ImageSliderBar jetpackAirDashCooldownBar;

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

    #region Jetpack

    [Header("Jetpack")]
    // Settings
    [Header("Settings")]
    [SerializeField] private bool hasJetpack;
    [SerializeField] private float jetpackForce = 25.0f;
    [SerializeField] private float maxJetpackDuration = 3f;
    [SerializeField] private float jetpackAirDashPower = 10f;
    [SerializeField] private float airDashVelocityFalloffSpeed = 1f;
    [SerializeField] private float jetpackAirDashCooldown = 5f;
    [SerializeField] private float jetpackAirDashCooldownTimer;
    [SerializeField] private Color jetpackAirDashAvailableColor;
    [SerializeField] private Color jetpackAirDashUnavailableColor;
    [SerializeField] private AudioClipContainer airDashClip;
    private bool CanUseAirDash
    {
        get
        {
            return jetpackAirDashCooldownTimer <= 0 && flying;
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

    private new void Awake()
    {
        // Set Fuel
        fuelStore.Reset();

        // Update UI
        jetpackDisplay.Set(0, maxJetpackDuration);

        base.Awake();
    }

    private void Start()
    {
        // Get References
        controller = GetComponent<CharacterController>();

        cameraTransform = Camera.main.transform;

        // Fetch the Input Manager Singleton
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Jump.performed += Jump;
        inputManager.PlayerInputActions.Player.Sprint.performed += TryJetpackAirDash;

        SetHPBar();
    }

    private void OnDestroy()
    {
        // Remove input actions
        inputManager.PlayerInputActions.Player.Jump.performed -= Jump;
        inputManager.PlayerInputActions.Player.Sprint.performed -= TryJetpackAirDash;
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

        // Control Air Dash
        // if grounded, remove velocity
        if (isGrounded)
        {
            jetpackAirDashVelocity = Vector3.zero;
        }
        // if velocity is not 0, move towards to 0
        if (jetpackAirDashVelocity != Vector3.zero)
        {
            jetpackAirDashVelocity = Vector3.Lerp(jetpackAirDashVelocity, Vector3.zero, airDashVelocityFalloffSpeed * Time.deltaTime);
        }
        // Set UI for Air Dash Cooldown
        jetpackAirDashCooldownBar.Set(jetpackAirDashCooldownTimer, jetpackAirDashCooldown);
        jetpackAirDashCooldownBar.SetColor(CanUseAirDash ? jetpackAirDashAvailableColor : jetpackAirDashUnavailableColor);

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
        if (jetpackAirDashCooldownTimer > 0)
        {
            jetpackAirDashCooldownTimer -= Time.deltaTime;
        }

        // Debug.Log("Velocity: " + playerVelocity);

        controller.Move(playerVelocity * Time.deltaTime);
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
            fuelStore.CurrentFuel -= Time.deltaTime * fuelConsumptionRate;
            if (fuelStore.CurrentFuel < 0) fuelStore.CurrentFuel = 0;

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

    private void TryJetpackAirDash(InputAction.CallbackContext ctx)
    {
        if (!CanUseAirDash)
        {
            if (jetpackAirDashCooldownTimer > 0)
            {
                Debug.Log("Air Dash On Cooldown; Can't use Air Dash");
                return;
            }
            if (!flying)
            {
                Debug.Log("Not Flying; Can't use Air Dash");
                return;
            }
        }

        Debug.Log("Activate Air Dash");

        // Audio
        airDashClip.PlayOneShot(source);

        // Add Force
        Vector3 dashForce = movementVector * jetpackAirDashPower;
        jetpackAirDashVelocity += dashForce;

        // Set cooldown
        jetpackAirDashCooldownTimer = jetpackAirDashCooldown;
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
        SetHPBar();
    }

    private void SetHPBar()
    {
        playerHpBar.Set(currentHealth, maxHealth);
    }
}
