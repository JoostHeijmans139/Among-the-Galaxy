using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Handles controller navigation for the crafting menu by managing button selection
/// </summary>
public class CraftingMenuNavigationController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private GameObject firstSelectedButton;
    
    [SerializeField] private Transform buttonParent;
    
    [SerializeField] private Button confirmButton;

    private EventSystem eventSystem;
    private GameObject lastSelectedButton;
    private GameObject currentlyTrackedSelection; // Track selection every frame
    private float lastClickTime = 0f;
    private const float CLICK_COOLDOWN = 0.3f; // Don't auto-reselect right after a click

    private void Awake()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found!");
        }
    }

    private void OnEnable()
    {
        // Wait one frame to ensure all UI elements are initialized
        StartCoroutine(SelectFirstButtonDelayed());
    }

    private IEnumerator SelectFirstButtonDelayed()
    {
        // Small delay to ensure UI is fully initialized
        yield return null;
        yield return null;
        
        SelectFirstButton();
    }

    /// <summary>
    /// Selects the first available button for controller navigation
    /// </summary>
    public void SelectFirstButton()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem is null in CraftingMenuNavigationController!");
                return;
            }
        }

        // If a specific first button is set, use that
        if (firstSelectedButton != null && firstSelectedButton.activeInHierarchy)
        {
            GameObject buttonToSelect = firstSelectedButton;
            
            // Check if it's a button and is interactable
            Button button = buttonToSelect.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                eventSystem.SetSelectedGameObject(buttonToSelect);
                Debug.Log($"Selected first button: {buttonToSelect.name}");
                return;
            }
        }

        // Otherwise, find the first active and interactable button in the button parent
        if (buttonParent != null)
        {
            foreach (Transform child in buttonParent)
            {
                Button button = child.GetComponent<Button>();
                if (button != null && button.interactable && child.gameObject.activeInHierarchy)
                {
                    eventSystem.SetSelectedGameObject(child.gameObject);
                    Debug.Log($"Selected button from parent: {child.name}");
                    return;
                }
            }
        }

        // Fallback: Try to select the confirm button
        if (confirmButton != null && confirmButton.interactable && confirmButton.gameObject.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(confirmButton.gameObject);
            Debug.Log($"Selected confirm button as fallback");
        }
        else
        {
            Debug.LogWarning("No selectable button found in crafting menu!");
        }
    }

    /// <summary>
    /// Call this to refresh the selection when button states change
    /// </summary>
    public void RefreshSelection()
    {
        // If nothing is currently selected, select the first button
        if (eventSystem != null && eventSystem.currentSelectedGameObject == null)
        {
            SelectFirstButton();
        }
    }

    private float timeSinceSelectionLost = 0f;
    private const float RESELECT_DELAY = 0.3f; // Increased delay to avoid interfering with clicks

    private void Update()
    {
        if (!gameObject.activeInHierarchy || eventSystem == null)
            return;

        GameObject currentSelected = eventSystem.currentSelectedGameObject;
        
        // Track selection FIRST before any input processing
        // This gives us a reliable snapshot of what was selected before EventSystem changes it
        if (currentSelected != null)
        {
            currentlyTrackedSelection = currentSelected;
        }
        
        // Debug: Show what's currently selected
        if (Input.GetKeyDown(KeyCode.D) || (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame))
        {
            if (currentSelected != null)
            {
                Debug.Log($"[DEBUG] EventSystem currently selected: {currentSelected.name}");
                CraftingButton cb = currentSelected.GetComponent<CraftingButton>();
                if (cb != null)
                {
                    Debug.Log($"[DEBUG] Selected button recipe: {cb.recipe.itemName}");
                }
            }
            else
            {
                Debug.Log("[DEBUG] EventSystem has no selected object!");
            }
        }
        
        // Detect button clicks and trigger them on the TRACKED selection (not current)
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            lastClickTime = Time.unscaledTime;
            Debug.Log($"[NavigationController] Button click detected. CurrentlyTracked: {(currentlyTrackedSelection != null ? currentlyTrackedSelection.name : "null")}, EventSystem.current: {(currentSelected != null ? currentSelected.name : "null")}");
            
            // Use the tracked selection, not the current one (which might have already changed)
            if (currentlyTrackedSelection != null)
            {
                CraftingButton craftingBtn = currentlyTrackedSelection.GetComponent<CraftingButton>();
                if (craftingBtn != null)
                {
                    Debug.Log($"[NavigationController] Triggering click on tracked button: {craftingBtn.recipe.itemName}");
                    craftingBtn.TriggerClick();
                }
                else
                {
                    // Might be the confirm button or other UI element
                    Button btn = currentlyTrackedSelection.GetComponent<Button>();
                    if (btn != null)
                    {
                        Debug.Log($"[NavigationController] Triggering onClick for: {currentlyTrackedSelection.name}");
                        btn.onClick.Invoke();
                    }
                }
            }
            else
            {
                Debug.LogWarning("[NavigationController] No tracked selection available for button press!");
            }
        }
        
        // Track the last valid selection
        if (currentSelected != null)
        {
            lastSelectedButton = currentSelected;
            timeSinceSelectionLost = 0f;
        }
        else
        {
            // No selection - but don't reselect immediately after a click
            float timeSinceClick = Time.unscaledTime - lastClickTime;
            if (timeSinceClick < CLICK_COOLDOWN)
            {
                // Too soon after a click, don't reselect yet
                timeSinceSelectionLost = 0f;
                return;
            }
            
            // No selection - wait before reselecting
            timeSinceSelectionLost += Time.unscaledDeltaTime;
            
            if (timeSinceSelectionLost >= RESELECT_DELAY)
            {
                Debug.Log("Selection lost, attempting to reselect...");
                
                // Try to reselect the last button first
                if (lastSelectedButton != null && lastSelectedButton.activeInHierarchy)
                {
                    Button btn = lastSelectedButton.GetComponent<Button>();
                    if (btn != null && btn.interactable)
                    {
                        eventSystem.SetSelectedGameObject(lastSelectedButton);
                        Debug.Log($"Reselected last button: {lastSelectedButton.name}");
                        timeSinceSelectionLost = 0f;
                        return;
                    }
                }
                
                // Fall back to first button
                SelectFirstButton();
                timeSinceSelectionLost = 0f;
            }
        }
    }
}
