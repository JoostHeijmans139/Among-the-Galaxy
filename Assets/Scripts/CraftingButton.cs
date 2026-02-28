using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Script for each recipe that each Button has in crafting button list
public class CraftingButton : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    public CraftingRecipe recipe;
    public Button button;
    
    private bool isSelected = false;
    private int lastClickFrame = -1; // Track frame number to prevent duplicate clicks
    private int lastSelectedFrame = -1; // Track when this button was last selected

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        lastSelectedFrame = Time.frameCount;
        Debug.Log($"Button selected: {recipe.itemName} at frame {Time.frameCount}");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        Debug.Log($"Button deselected: {recipe.itemName} at frame {Time.frameCount}");
    }

    private void OnDisable()
    {
        isSelected = false;
    }

    // Public method called by CraftingMenuNavigationController when A button is pressed
    public void TriggerClick()
    {
        // Prevent duplicate clicks in the same frame
        if (lastClickFrame == Time.frameCount)
        {
            Debug.Log($"[CraftingButton] Duplicate click prevented for {recipe.itemName} in frame {Time.frameCount}");
            return;
        }
        
        lastClickFrame = Time.frameCount;
        OnClickInternal();
    }

    // Internal method that actually performs the click action (no frame check)
    private void OnClickInternal()
    {
        Debug.Log($"[CraftingButton] OnClickInternal called - Button name: {gameObject.name}, Recipe: {recipe.itemName}, Frame: {Time.frameCount}");
        
        if (CraftingMenuUI.Instance == null)
        {
            Debug.LogError("[CraftingButton] CraftingMenuUI.Instance is null!");
            return;
        }
        CraftingMenuUI.Instance.SelectRecipe(recipe);
    }

    // Public OnClick for button.onClick listener (includes frame check)
    private void OnClick()
    {
        // Prevent duplicate clicks in the same frame
        if (lastClickFrame == Time.frameCount)
        {
            Debug.Log($"[CraftingButton] Duplicate click prevented for {recipe.itemName} in frame {Time.frameCount}");
            return;
        }
        
        lastClickFrame = Time.frameCount;
        OnClickInternal();
    }
    
    // Handle mouse clicks
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick called for recipe: {recipe.itemName}");
        OnClick();
    }
    
    // Explicitly handle Submit input
    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log($"OnSubmit called for recipe: {recipe.itemName}");
        if (button.interactable)
        {
            OnClick();
        }
    }
}
