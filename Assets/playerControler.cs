using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using StarterAssets;

public class playerControler : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    // Component references
    private CharacterController controller;
    private StarterAssetsInputs input;
    
    // Movement variables
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    
    /// <summary>
    /// Initialize component references and setup
    /// </summary>
    void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        
        if (input == null)
        {
            Debug.LogError("playerControler: StarterAssetsInputs component not found! Please add it to the Player.");
        }
        
        // Lock and hide cursor for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// Main update loop - handles input and movement
    /// </summary>
    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }
    
    /// <summary>
    /// Check if player is standing on ground using a sphere overlap
    /// </summary>
    private void HandleGroundCheck()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            // Fallback ground check using CharacterController
            isGrounded = controller.isGrounded;
        }

        if (isGrounded)
        {
            Debug.Log("GROUNDED");
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep player grounded
        }
    }
    
    /// <summary>
    /// Handle player movement with WASD/arrow keys and sprint with Shift
    /// </summary>
    private void HandleMovement()
    {
        if (input == null) return;
        
        // Get input from StarterAssetsInputs
        Vector2 moveInput = input.move;
        
        // Determine current speed (sprint or walk)
        currentSpeed = input.sprint ? sprintSpeed : walkSpeed;
        
        // Calculate movement direction relative to player rotation
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Handle jump input and apply jump force
    /// </summary>
    private void HandleJump()
    {
        if (input != null && input.jump)
        {
            if (isGrounded)
            {
                // Calculate jump velocity using physics formula: v = sqrt(h * -2 * g)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            // Always reset jump input to prevent queuing
            input.jump = false;
        }
    }
    
    /// <summary>
    /// Apply gravity to player every frame
    /// </summary>
    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Unlock cursor when ESC is pressed (for debugging/menus)
    /// </summary>
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
