using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

/// <summary>
/// This class manages the selection of interactable objects in the scene.
/// It uses raycasting to detect objects the player is looking at and displays
/// interaction information if the object is interactable.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    [Header("UI References")]
    public GameObject InteractionInfo;
    private TMP_Text interactionInfoText;


    [Header("Raycast Settings")]
    public Camera playerCamera;
    public float maxDistance = 5f;
    public LayerMask interactableLayers = -1;

    [Header("Debug")]
    public bool showDebugRay = true;
    
    public bool onTarget;
    private StarterAssetsInputs playerInputs;
    private InteractableObject currentInteractable;

    /// <summary>
    /// Make singleton
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Initialize component references
    /// </summary>
    private void Start()
    {
        // Get interaction info text component
        if (InteractionInfo != null)
        {
            interactionInfoText = InteractionInfo.GetComponent<TMP_Text>();
            InteractionInfo.SetActive(false);
        }

        onTarget = false;

        // Find player camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("SelectionManager: No camera found! Please assign a camera.");
            }
        }

        // Assign player and its StarterAssetsInputs
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInputs = player.GetComponent<StarterAssetsInputs>();
        }
        else
        {
            Debug.LogWarning("SelectionManager: Player not found. Input handling may not work.");
        }
    }

    /// <summary>
    /// Update loop - performs raycasting and handles interaction
    /// </summary>
    void Update()
    {
        if (playerCamera == null)
            return;

        // Perform raycast from camera center
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Debug visualization
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow);
        }

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayers))
        {
            // Check if hit object is interactable
            InteractableObject interactable = hit.transform.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                onTarget = true;

                // Show basic interaction info
                if (InteractionInfo != null && interactionInfoText != null)
                {
                    interactionInfoText.text = interactable.GetItemName();
                    InteractionInfo.SetActive(true);
                }

                // Handle interaction input
                if (playerInputs != null && playerInputs.attack)
                {
                    interactable.OnPunchOrShoot();
                    playerInputs.attack = false;
                }
                // Fallback if StarterAssetsInputs not available
                else if (Input.GetMouseButtonDown(0)) 
                {
                    interactable.OnPunchOrShoot();
                }
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }

        // Reset attack input
        if (playerInputs != null)
        {
            playerInputs.attack = false;
        }
    }

    /// <summary>
    /// Clear current target and hide UI
    /// </summary>
    private void ClearTarget()
    {
        currentInteractable = null;
        onTarget = false;

        if (InteractionInfo != null)
        {
            InteractionInfo.SetActive(false);
        }
    }

    /// <summary>
    /// Get the currently targeted interactable object
    /// </summary>
    public InteractableObject GetCurrentInteractable()
    {
        return currentInteractable;
    }
}
