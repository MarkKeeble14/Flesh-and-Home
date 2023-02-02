using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Movement
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float airStrafeSpeed = .5f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float mass = 3.0f;
    [SerializeField] private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private bool hasPlayedLandedSound;

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
    private bool flying;

    // Fuel
    [SerializeField] private FuelStore fuelStore;
    [SerializeField] private float fuelConsumptionRate = 1f;
    [SerializeField] private ParticleSystem jetpackParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource jetpackAudioSource;
    [SerializeField] private AudioClip jetpackStartClip;
    [SerializeField] private AudioClip jetpackEndClip;

    [Header("References")]
    [SerializeField] private JetpackDisplay jetpackDisplay;

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

    [Header("References")]
    [SerializeField] private AudioSource source;
    private CharacterController controller;
    private InputManager inputManager;
    private Transform cameraTransform;

    private void Awake()
    {
        // Set Fuel
        fuelStore.Reset();
    }

    private void Start()
    {
        // Get References
        controller = GetComponent<CharacterController>();

        inputManager = InputManager._Instance;
        cameraTransform = Camera.main.transform;

        // Fetch the Input Manager Singleton
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Jump.performed += Jump;
    }

    void Update()
    {
        // Set collision states
        isGrounded = Physics.CheckSphere(groundCheckPosition.position, environmentCollisionCheckRadius, environmentCollisions);
        isBonking = Physics.CheckSphere(bonkCheckPosition.position, environmentCollisionCheckRadius, environmentCollisions);

        if (isGrounded && playerVelocity.y < 0)
        {
            if (!hasPlayedLandedSound)
            {
                source.PlayOneShot(landClip);
                hasPlayedLandedSound = true;
            }
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * MoveSpeed);

        // Enable footsteps audio source when walking
        footstepsSource.enabled = isGrounded && move != Vector3.zero;

        // Changes the height position of the player..
        if (inputManager.PlayerJumpedThisFrame() && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -mass * gravityValue * Time.deltaTime);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        // Debug.Log("Velocity: " + playerVelocity);

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            // Debug.Log("Jump");
            hasPlayedLandedSound = false;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -mass * gravityValue);

            source.PlayOneShot(jumpClip);
        }
        else if (hasJetpack && !flying)
        {
            hasPlayedLandedSound = false;
            StartCoroutine(Jetpack());
        }
    }

    private IEnumerator Jetpack()
    {
        // Debug.Log("Start Jetpack");

        flying = true;
        float timer = 0;

        // Make noise
        source.PlayOneShot(jetpackStartClip);
        jetpackAudioSource.enabled = true;
        jetpackParticles.Play();

        // Player must be holding the button, have time left, and also must have fuel left to burn
        while (inputManager.PlayerInputActions.Player.Jump.IsPressed() && timer < maxJetpackDuration && fuelStore.CurrentFuel > 0)
        {
            // Update timer
            timer += Time.deltaTime;

            // Update UI
            jetpackDisplay.Set(timer, maxJetpackDuration);

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
}
