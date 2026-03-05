using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Handles controller navigation for the Game Over screen
/// </summary>
public class GameOverNavigationController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Button restartButton;

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
        // Use realtime wait since Time.timeScale = 0 during game over
        yield return new WaitForSecondsRealtime(0.1f);
        
        SelectFirstButton();
    }

    /// <summary>
    /// Selects the restart button for controller navigation
    /// </summary>
    public void SelectFirstButton()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem is null!");
                return;
            }
        }

        // Select the restart button
        if (restartButton != null && restartButton.interactable && restartButton.gameObject.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(restartButton.gameObject);
            Debug.Log($"Selected restart button! Current selected: {eventSystem.currentSelectedGameObject?.name}");
        }
        else
        {
            Debug.LogWarning($"Restart button not selectable - Button null: {restartButton == null}, Interactable: {restartButton?.interactable}, Active: {restartButton?.gameObject.activeInHierarchy}");
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
        
        // Use old Input system because Time.timeScale = 0 prevents new Input System from working
        // Check for gamepad button (joystick button 0 = A/Cross on most controllers) or keyboard
        bool buttonPressed = Input.GetKeyDown(KeyCode.JoystickButton0) || 
                            Input.GetKeyDown(KeyCode.Return) || 
                            Input.GetKeyDown(KeyCode.KeypadEnter);
        
        // Detect button clicks and trigger them on the TRACKED selection (not current)
        if (buttonPressed)
        {
            lastClickTime = Time.unscaledTime;
            Debug.Log($"[GameOverNavigation] Button click detected. CurrentlyTracked: {(currentlyTrackedSelection != null ? currentlyTrackedSelection.name : "null")}, EventSystem.current: {(currentSelected != null ? currentSelected.name : "null")}");
            
            // Use the tracked selection, not the current one (which might have already changed)
            if (currentlyTrackedSelection != null)
            {
                // Just trigger the button directly
                Button btn = currentlyTrackedSelection.GetComponent<Button>();
                if (btn != null)
                {
                    Debug.Log($"[GameOverNavigation] Found button component. Button.interactable: {btn.interactable}, onClick listener count: {btn.onClick.GetPersistentEventCount()}");
                    Debug.Log($"[GameOverNavigation] Triggering onClick for: {currentlyTrackedSelection.name}");
                    btn.onClick.Invoke();
                }
                else
                {
                    Debug.LogError($"[GameOverNavigation] No Button component found on {currentlyTrackedSelection.name}!");
                }
            }
            else
            {
                Debug.LogWarning("[GameOverNavigation] No tracked selection available for button press!");
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
