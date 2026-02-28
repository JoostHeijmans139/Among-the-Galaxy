using UnityEngine;
using UnityEngine.InputSystem;

// Class that switches between gameplay and Crafting screen
public class CraftingMenuController : MonoBehaviour
{
    public static CraftingMenuController Instance;

    [SerializeField] private GameObject craftingUI;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CraftingMenuNavigationController navigationController;

    private bool isOpen;

    private void Awake()
    {
        Instance = this;
        craftingUI.SetActive(false);
    }
    public void ToggleCrafting()
    {
        isOpen = !isOpen;
        craftingUI.SetActive(isOpen);

        if (isOpen)
        {
            playerInput.SwitchCurrentActionMap("UI");
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
            playerInput.SwitchCurrentActionMap("Player");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
