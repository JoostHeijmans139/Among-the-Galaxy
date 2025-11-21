using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

/// <summary>
/// This class manages the selection of interactable objects in the scene.
/// It uses raycasting to detect objects under the mouse cursor and displays.
/// Then it shows interaction information if the object is interactable.
public class SelectionManager : MonoBehaviour
{

    public static SelectionManager Instance { get; set; }

    public GameObject InteractionInfo;
    private TMP_Text interactionInfoText;

    public Camera playerCamera;
    public float maxDistance = 5f;

    public bool onTarget;

    private StarterAssetsInputs playerInputs;

    // Interaction info text set to the text mesh pro component
    private void Start()
    {
        interactionInfoText = InteractionInfo.GetComponent<TMP_Text>();
        InteractionInfo.SetActive(false);

        onTarget = false;

        // Assign player and its StarterAssetsInputs
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInputs = player.GetComponent<StarterAssetsInputs>();
        }
        else
        {
            Debug.Log("player not found");
        }
    }

    // Make singleton
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

    void Update()
    {
        Vector3 rayOrigin = playerCamera.transform.position + playerCamera.transform.forward * 0.5f;
        Ray ray = new Ray(rayOrigin, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                interactionInfoText.text = interactable.GetItemName();
                InteractionInfo.SetActive(true);

                // Only call punch/shoot if looking at an interactable and input is pressed
                if (playerInputs != null && playerInputs.attack)
                {
                    interactable.OnPunchOrShoot();
                }
            }
            else
            {
                InteractionInfo.SetActive(false);
            }
        }
        else
        {
            InteractionInfo.SetActive(false);
        }

        playerInputs.attack = false;
    }

}
