using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Class that switches between gameplay and Crafting screen
public class CraftingMenuController : MonoBehaviour
{
    public static CraftingMenuController Instance;

    [SerializeField] private GameObject craftingUI;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CraftingMenuNavigationController navigationController;

    private bool isOpen;
    private float lastToggleTime = -999f;
    private const float TOGGLE_COOLDOWN = 0.5f; // Prevent rapid toggling

    private void Awake()
    {
        Instance = this;
        craftingUI.SetActive(false);
        
        // Find PlayerInput if not assigned
        EnsurePlayerInputReference();
    }
    
    private void EnsurePlayerInputReference()
    {
        // If playerInput reference is lost (e.g., after scene reload), find it again
        if (playerInput == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerInput = playerObj.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    Debug.Log("[CraftingMenu] Found PlayerInput component on Player");
                }
                else
                {
                    Debug.LogWarning("[CraftingMenu] Player object found but no PlayerInput component!");
                }
            }
            else
            {
                Debug.LogWarning("[CraftingMenu] Could not find Player object!");
            }
        }
        
        // Ensure the Player action map is active at start
        if (playerInput != null)
        {
            if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != "Player")
            {
                try
                {
                    playerInput.SwitchCurrentActionMap("Player");
                    Debug.Log("[CraftingMenu] Initialized PlayerInput to Player action map");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[CraftingMenu] Could not switch to Player action map on start: {e.Message}");
                }
            }
        }
    }
    public void ToggleCrafting()
    {
        // Prevent rapid toggling
        if (Time.time - lastToggleTime < TOGGLE_COOLDOWN)
        {
            return;
        }
        
        // Ensure we have PlayerInput reference (important after scene reload)
        EnsurePlayerInputReference();
        
        lastToggleTime = Time.time;
        isOpen = !isOpen;
        craftingUI.SetActive(isOpen);

        if (isOpen)
        {
            // Switch to UI action map to disable player controls (jump, move, etc.)
            if (playerInput != null)
            {
                try
                {
                    playerInput.SwitchCurrentActionMap("UI");
                    Debug.Log("[CraftingMenu] Switched to UI action map");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CraftingMenu] Failed to switch to UI action map: {e.Message}");
                    // Fallback: deactivate input if action map switch fails
                    playerInput.DeactivateInput();
                }
            }
            else
            {
                Debug.LogError("[CraftingMenu] PlayerInput is null! Cannot switch action maps.");
            }
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Enable controller navigation by selecting first button
            if (navigationController != null)
            {
                navigationController.SelectFirstButton();
            }
        }
        else
        {
            // Switch back to Player action map with delay to clear input
            StartCoroutine(SwitchToPlayerWithDelay());
        }
    }
    
    private IEnumerator SwitchToPlayerWithDelay()
    {
        // Wait to ensure button release is fully processed
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.2f);
        
        // Switch back to Player action map
        if (playerInput != null)
        {
            try
            {
                playerInput.SwitchCurrentActionMap("Player");
                Debug.Log("[CraftingMenu] Switched back to Player action map");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CraftingMenu] Failed to switch to Player action map: {e.Message}");
                // Fallback: reactivate input if action map switch fails
                playerInput.ActivateInput();
            }
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
