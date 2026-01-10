// hello
using UnityEngine;
using StarterAssets;

/// <summary>
/// Handles first-person camera rotation and looking at objects.
/// Attach this to a Camera that is a child of the player object.
/// </summary>
public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float verticalLookLimit = 80f;
    
    [Header("Camera Offset")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.6f, 0f); // Eye height
    
    private Transform playerBody;
    private StarterAssetsInputs input;
    private float xRotation = 0f;
    
    void Start()
    {
        // Get parent transform (player body)
        playerBody = transform.parent;
        
        if (playerBody == null)
        {
            Debug.LogError("FirstPersonCamera must be a child of the player object!");
            enabled = false;
            return;
        }
        
        // Get input component from parent (player)
        input = playerBody.GetComponent<StarterAssetsInputs>();
        
        if (input == null)
        {
            Debug.LogError("FirstPersonCamera: StarterAssetsInputs not found on player! Please add it.");
            enabled = false;
            return;
        }
        
        // Set initial camera position
        transform.localPosition = cameraOffset;
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleMouseLook();
        
        // Allow unlocking cursor with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }
    }
    
    /// <summary>
    /// Handle mouse look for first-person camera control
    /// </summary>
    private void HandleMouseLook()
    {
        if (input == null || playerBody == null)
            return;
            
        // Only rotate when cursor is locked
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        
        // Get mouse input from StarterAssetsInputs
        Vector2 lookInput = input.look;
        
        // Apply sensitivity
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        // Vertical rotation (look up/down) - applied to camera
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Horizontal rotation (look left/right) - applied to player body
        playerBody.Rotate(Vector3.up * mouseX);
    }
    
    /// <summary>
    /// Toggle cursor lock state
    /// </summary>
    private void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    /// <summary>
    /// Get the camera component
    /// </summary>
    public Camera GetCamera()
    {
        return GetComponent<Camera>();
    }
}
