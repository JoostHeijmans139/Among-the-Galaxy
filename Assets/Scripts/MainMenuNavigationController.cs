using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles controller navigation for the Main Menu
/// </summary>
public class MainMenuNavigationController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Button firstSelectedButton; // The first button to select (e.g., Generate World)
    [SerializeField] private Transform buttonParent; // Optional: parent containing all buttons

    private EventSystem eventSystem;
    private GameObject lastSelectedButton;
    private GameObject currentlyTrackedSelection; // Track selection every frame
    private float lastClickTime = 0f;
    private const float CLICK_COOLDOWN = 0.3f;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("[MainMenuNavigation] EventSystem not found!");
        }
    }

    private void OnEnable()
    {
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
    /// Selects the first button for controller navigation
    /// </summary>
    public void SelectFirstButton()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError("[MainMenuNavigation] EventSystem is null!");
                return;
            }
        }

        // Try to select the specified first button
        if (firstSelectedButton != null && firstSelectedButton.interactable && firstSelectedButton.gameObject.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject);
            Debug.Log($"[MainMenuNavigation] Selected first button: {firstSelectedButton.name}");
            return;
        }

        // Fallback: find first active button in button parent
        if (buttonParent != null)
        {
            foreach (Transform child in buttonParent)
            {
                Button button = child.GetComponent<Button>();
                if (button != null && button.interactable && child.gameObject.activeInHierarchy)
                {
                    eventSystem.SetSelectedGameObject(child.gameObject);
                    Debug.Log($"[MainMenuNavigation] Selected button from parent: {child.name}");
                    return;
                }
            }
        }

        // Final fallback: search for ANY active and interactable button in the entire scene
        Button[] allButtons = FindObjectsOfType<Button>();
        Debug.Log($"[MainMenuNavigation] Searching {allButtons.Length} buttons in scene...");
        foreach (Button button in allButtons)
        {
            if (button.interactable && button.gameObject.activeInHierarchy)
            {
                // Additional check: make sure the button is actually visible (parent chain is active)
                Transform current = button.transform;
                bool isVisible = true;
                while (current != null)
                {
                    if (!current.gameObject.activeSelf)
                    {
                        isVisible = false;
                        break;
                    }
                    current = current.parent;
                }
                
                if (isVisible)
                {
                    eventSystem.SetSelectedGameObject(button.gameObject);
                    Debug.Log($"[MainMenuNavigation] Selected button from scene search: {button.name}");
                    return;
                }
            }
        }

        Debug.LogWarning("[MainMenuNavigation] No selectable button found!");
    }

    private float timeSinceSelectionLost = 0f;
    private const float RESELECT_DELAY = 0.05f; // Very quick reselection for menu navigation
    private bool wasSelectedLastFrame = false;

    private void Update()
    {
        if (!gameObject.activeInHierarchy || eventSystem == null)
            return;

        GameObject currentSelected = eventSystem.currentSelectedGameObject;
        bool isSelectedThisFrame = currentSelected != null;
        
        // Track selection FIRST before any input processing
        if (isSelectedThisFrame)
        {
            currentlyTrackedSelection = currentSelected;
            lastSelectedButton = currentSelected;
            timeSinceSelectionLost = 0f;
            wasSelectedLastFrame = true;
        }
        else
        {
            // Selection was just lost this frame - immediately try to reselect
            if (wasSelectedLastFrame)
            {
                Debug.Log("[MainMenuNavigation] Selection just lost! Immediate reselect attempt...");
                wasSelectedLastFrame = false;
                // Small delay before reselecting
                timeSinceSelectionLost = 0f;
            }
            
            // No selection - but don't reselect immediately after a click
            float timeSinceClick = Time.unscaledTime - lastClickTime;
            if (timeSinceClick < CLICK_COOLDOWN)
            {
                timeSinceSelectionLost = 0f;
                return;
            }
            
            // No selection - wait before reselecting
            timeSinceSelectionLost += Time.unscaledDeltaTime;
            
            if (timeSinceSelectionLost >= RESELECT_DELAY)
            {
                Debug.Log("[MainMenuNavigation] Selection lost for too long, attempting to reselect...");
                SelectFirstButton();
                timeSinceSelectionLost = 0f;
            }
        }
        
        // Use old Input API since there's no PlayerInput component in main menu
        // JoystickButton0 = A/Cross button on most controllers
        bool buttonPressed = Input.GetKeyDown(KeyCode.JoystickButton0) || 
                            Input.GetKeyDown(KeyCode.Return) || 
                            Input.GetKeyDown(KeyCode.KeypadEnter);
        
        // Detect button clicks and trigger them on the TRACKED selection
        if (buttonPressed)
        {
            lastClickTime = Time.unscaledTime;
            Debug.Log($"[MainMenuNavigation] Button click detected. Tracked: {(currentlyTrackedSelection != null ? currentlyTrackedSelection.name : "null")}");
            
            if (currentlyTrackedSelection != null)
            {
                Button btn = currentlyTrackedSelection.GetComponent<Button>();
                if (btn != null && btn.interactable)
                {
                    Debug.Log($"[MainMenuNavigation] Triggering onClick for: {currentlyTrackedSelection.name}");
                    btn.onClick.Invoke();
                    
                    // After clicking, the menu might change - force reselection after a short delay
                    StartCoroutine(ReselectAfterMenuChange());
                }
            }
            else
            {
                Debug.LogWarning("[MainMenuNavigation] No tracked selection available for button press!");
            }
        }
    }
    
    /// <summary>
    /// Waits for menu to change after button click, then forces reselection
    /// </summary>
    private IEnumerator ReselectAfterMenuChange()
    {
        // Wait for the menu to change (buttons to be hidden/shown)
        yield return new WaitForSecondsRealtime(0.1f);
        
        Debug.Log("[MainMenuNavigation] Forcing reselection after menu change...");
        
        // Force clear the cooldown so reselection can happen immediately
        lastClickTime = Time.unscaledTime - CLICK_COOLDOWN;
        timeSinceSelectionLost = RESELECT_DELAY;
        
        // Immediately try to select a button
        SelectFirstButton();
    }
}
