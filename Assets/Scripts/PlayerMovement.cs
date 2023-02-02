using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 1f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float airStrafeSpeed = .5f;
    [SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    [Header("Jetpack")]
    // Settings
    [SerializeField] private bool hasJetpack;
    [SerializeField] private float jetpackForce = 25.0f;
    [SerializeField] private float maxJetpackDuration = 3f;
    [SerializeField] private FuelStore fuelStore;
    [SerializeField] private float fuelConsumptionRate = 1f;
    // Polish
    [SerializeField] private ParticleSystem jetpackParticles;
    [SerializeField] private AudioSource jetpackAudioSource;
    [SerializeField] private AudioClip jetpackStartClip;
    [SerializeField] private AudioClip jetpackEndClip;
    private bool flying;

    // References
    [Header("References")]
    [SerializeField] private AudioSource source;
    private InputManager inputManager;
    private Rigidbody rb;

    private float MoveSpeed
    {
        get
        {
            return flying ? airStrafeSpeed : (baseMoveSpeed * (IsSprinting ? sprintSpeed : 1));
        }
    }

    private bool IsSprinting
    {
        get
        {
            return inputManager.PlayerInputActions.Player.Sprint.IsPressed();
        }
    }

    private void Awake()
    {
        // Get References
        rb = GetComponent<Rigidbody>();
        fuelStore.Reset();
    }

    private void Start()
    {
        // Fetch the Input Manager Singleton
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Jump.performed += Jump;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            // Debug.Log("Jump");
            source.PlayOneShot(jumpClip);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else if (hasJetpack && !flying)
        {
            StartCoroutine(Jetpack());
        }
    }

    private void SetIsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector2.down, out hit, 1.5f, groundLayer);
        if (!isGrounded && hit.collider != null)
        {
            source.PlayOneShot(landClip);
        }
        isGrounded = hit.collider != null;
    }

    void Update()
    {
        SetIsGrounded();

        // Get Player Input
        Vector2 moveVector = inputManager.PlayerInputActions.Player.Move.ReadValue<Vector2>();

        // Enable footsteps audio source when walking
        footstepsSource.enabled = isGrounded && moveVector != Vector2.zero;

        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y);
        move = Camera.main.transform.forward * move.z + Camera.main.transform.right * move.x;
        move.y = 0;
        transform.Translate(move * MoveSpeed * Time.deltaTime);

        // Rotate to be the same direction as camera
        Vector3 cameraRotation = Camera.main.transform.eulerAngles;
        cameraRotation.x = 0;
        transform.eulerAngles = cameraRotation;
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

            // Use Fuel
            fuelStore.CurrentFuel -= Time.deltaTime * fuelConsumptionRate;
            if (fuelStore.CurrentFuel < 0) fuelStore.CurrentFuel = 0;

            // Add force to move
            rb.AddForce(Vector3.up * jetpackForce * Time.deltaTime, ForceMode.Impulse);
            yield return null;
        }

        // Make noise
        source.PlayOneShot(jetpackEndClip);
        jetpackAudioSource.enabled = false;
        jetpackParticles.Stop();

        flying = false;
        // Debug.Log("End Jetpack");
    }
}

