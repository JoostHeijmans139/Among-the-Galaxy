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
    [Header("Settings")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float verticalClampAngle = 80f;
    [SerializeField] private float cameraSmoothTime = 0.1f;
    private float Pitch = 0f;       // Vertical rotation (pitch)
    private Vector2 currentMouseDelta; // Current smoothed mouse movement
    private Vector2 currentMouseDeltaVelocity; // Velocity reference for SmoothDamp
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component not found on the player object.");
            return;
        }

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
        UpdateCameraPosition();
        UpdateCameraRotation();
    }
    private void UpdateCameraPosition()
    {
        cameraTransform.position = transform.position + new Vector3(0, 10, -10)*Time.deltaTime;
    }

    private void UpdateCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        cameraTransform.Rotate(Vector3.up, -mouseX, Space.World);
        cameraTransform.Rotate(Vector3.right, mouseY);

    }
}
