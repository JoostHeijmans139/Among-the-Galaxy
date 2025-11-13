using System;
using UnityEngine;

public class playerControler : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity;
    private Rigidbody _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        { 
            Debug.LogError("Rigidbody component not found on the player object.");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Camera component not found on the player object.");
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }
    private void UpdateCameraPosition()
    {
        playerCamera.transform.position = transform.position + new Vector3(0, 10, -10)*Time.deltaTime;
    }

    private void UpdateCameraRotation()
    {
        var mouseX = Input.GetAxis("Mouse X")*mouseSensitivity*Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y")*mouseSensitivity*Time.deltaTime;
        playerCamera.transform.Rotate(Vector3.up, mouseX);
        playerCamera.transform.Rotate(Vector3.left, mouseY);
        
    }
}
