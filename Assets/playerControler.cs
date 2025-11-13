using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class playerControler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject playerCameraObject;
    private Camera playerCamera;
    private Transform cameraTransform;
    private Transform playerBody;
    private Rigidbody _rb;
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float verticalClampAngle = 80f;
    [SerializeField] private float cameraSmoothTime = 0.1f;
    private float Pitch = 0f;   
    [Header("Camera smoothing")] // Vertical rotation (pitch)
    private Vector2 currentMouseDelta; // Current smoothed mouse movement
    private Vector2 currentMouseDeltaVelocity; // Velocity reference for SmoothDamp
    [Header("Movement Settings")]
    private float horizontalInput;
    private float verticalInput;
    private Vector2 _rawInput;
    private Vector3 _moveDirection;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float moveSpeed = 5f;
    [Header("Ground Check Settings")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component not found on the player object.");
            return;
        }
        _rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        playerBody = this.TryGetComponent(out Transform playerTransform)? 
            playerTransform
            : throw new NullReferenceException("Player body transform is null.");
        playerCamera = playerCameraObject.TryGetComponent(out Camera cam) 
            ? cam 
            : throw new NullReferenceException("Player camera is null.");
        cameraTransform = playerCameraObject.TryGetComponent(out Transform camTransform)
            ? camTransform 
            : throw new NullReferenceException("Camera transform is null.");
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGroundedCheck();
        _rawInput = RawInputsToVector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Calculate movement direction relative to orientation (camera/player facing)
        _moveDirection = playerBody.forward * _rawInput.y + playerBody.right * _rawInput.x;
        //apply drag
        _rb.linearDamping = isGrounded ? groundDrag : 0f;
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void UpdateCameraPosition()
    {
        cameraTransform.position = transform.position + new Vector3(0, 10, -10)*Time.deltaTime;
    }

    private Vector2 RawInputsToVector2(float horizontalInput, float verticalInput)
    {
        Vector2 rawInput = new Vector2(horizontalInput, verticalInput);
        return rawInput;
    }

    private void UpdateCameraRotation()
    {
        //get raw mouse input
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //smooth the raw mouse input
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, mouseDelta, ref currentMouseDeltaVelocity, cameraSmoothTime);
        float mouseX = currentMouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = currentMouseDelta.y * mouseSensitivity * Time.deltaTime;
        playerBody.Rotate(Vector3.up * mouseX);
        Pitch -= mouseY;
        Pitch = Mathf.Clamp(Pitch, -verticalClampAngle, verticalClampAngle);
        cameraTransform.localRotation = Quaternion.Euler(Pitch, 0, 0);

    }

    private bool IsGroundedCheck()
    {
        bool result = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2f + 1f, groundMask);
        return result;
    }

    private void MovePlayer()
    {
        _rb.AddForce(_moveDirection.normalized * moveSpeed, ForceMode.Force);
        
    }
    private void Jump()
    {
        // Reset Y velocity
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
