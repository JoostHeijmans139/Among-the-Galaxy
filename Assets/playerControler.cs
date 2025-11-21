using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class playerControler : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    // Component references
    private CharacterController controller;
    
    // Movement variables
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    
    // Camera rotation variables
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    /// <summary>
    /// Initialize component references and setup
    /// </summary>
    void Start()
    {
        controller = GetComponent<CharacterController>();
            
        // Lock and hide cursor for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // If no camera assigned, try to find main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }
    }
    
    /// <summary>
    /// Main update loop - handles input and movement
    /// </summary>
    void Update()
    {
        cameraTransform.position = this.transform.position;
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleMouseLook();
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
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Determine current speed (sprint or walk)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Calculate movement direction relative to player rotation
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Handle jump input and apply jump force
    /// </summary>
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {

            // Calculate jump velocity using physics formula: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    /// <summary>
    /// Handle mouse look for first-person camera control
    /// </summary>
    /// <summary>
    /// Handle mouse look for first-person camera control
    /// </summary>
    private void HandleMouseLook()
    {
        if (cameraTransform == null) return;
        
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Accumulate horizontal rotation
        rotationY += mouseX;
        
        // Accumulate vertical rotation with clamping
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
        
        // Apply rotations using eulerAngles to force the rotation
        Vector3 playerRot = transform.eulerAngles;
        playerRot.y = rotationY;
        transform.eulerAngles = playerRot;
        
        // Apply camera rotation
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
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
