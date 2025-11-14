using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class manages the selection of interactable objects in the scene.
/// It uses raycasting to detect objects under the mouse cursor and displays.
/// Then it shows interaction information if the object is interactable.
public class SelectionManager : MonoBehaviour
{
    public GameObject InteractionInfo;
    private TMP_Text interactionInfoText;

    public Camera playerCamera;
    public float maxDistance = 5f;

    //interaction info text set to the text mesh pro component
    private void Start()
    {
        interactionInfoText = InteractionInfo.GetComponent<TMP_Text>();
        InteractionInfo.SetActive(false);
    }

    void Update()
    {
        // Offset the ray origin slightly in front of the camera to avoid hitting the player
        Vector3 rayOrigin = playerCamera.transform.position + playerCamera.transform.forward * 0.5f;
        Ray ray = new Ray(rayOrigin, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                interactionInfoText.text = interactable.GetItemName();
                InteractionInfo.SetActive(true);
            }
            else
            {
                InteractionInfo.SetActive(false);
            }
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, Color.green);
            InteractionInfo.SetActive(false);
        }
    }

}
