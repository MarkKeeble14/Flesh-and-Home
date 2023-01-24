using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float baseMoveSpeed = 1f;
    [SerializeField] private float sprintSpeed = 1.5f;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    // References
    private InputManager inputManager;
    private CharacterController controller;
    private float MoveSpeed
    {
        get
        {
            return baseMoveSpeed * (IsSprinting ? sprintSpeed : 1);
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
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Fetch the Input Manager Singleton
        inputManager = InputManager._Instance;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 moveVector = inputManager.PlayerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y);
        move = Camera.main.transform.forward * move.z + Camera.main.transform.right * move.x;
        move.y = 0;
        controller.Move(move * Time.deltaTime * MoveSpeed);

        /*
        // Changes the height position of the player..
        if (inputManager.PlayerInputActions.Player.Jump.IsPressed() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        */
        controller.Move(playerVelocity * Time.deltaTime);
    }
}

