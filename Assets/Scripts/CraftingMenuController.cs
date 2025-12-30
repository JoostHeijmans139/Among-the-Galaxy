using UnityEngine;
using UnityEngine.InputSystem;

// Class that switches between gameplay and Crafting screen
public class CraftingMenuController : MonoBehaviour
{
    public static CraftingMenuController Instance;

    [SerializeField] private GameObject craftingUI;
    [SerializeField] private PlayerInput playerInput;

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
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
