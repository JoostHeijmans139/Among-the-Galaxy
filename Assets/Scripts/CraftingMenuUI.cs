using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Code for generating the results area of a crafting recipe
public class CraftingMenuUI : MonoBehaviour
{
    public static CraftingMenuUI Instance;

    [Header("Details UI")]
    public TMP_Text itemNameText;
    public Image itemIcon;
    public Transform resourceListParent;
    public GameObject resourceRowPrefab;
    public Button confirmButton;

    private CraftingRecipe currentRecipe;

    private void Awake()
    {
        Instance = this;
        ClearDetails();

        // Add Confirm button functionality
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ConfirmCraft);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        currentRecipe = recipe;

        itemNameText.text = recipe.itemName;
        itemIcon.sprite = recipe.itemIcon;
        itemIcon.enabled = true;

        PopulateResourceList(recipe);
        UpdateConfirmButton();
    }

    private void PopulateResourceList(CraftingRecipe recipe)
    {
        foreach (Transform child in resourceListParent)
            Destroy(child.gameObject);

        foreach (var cost in recipe.costs)
        {
            var row = Instantiate(resourceRowPrefab, resourceListParent);
            row.GetComponent<ResourceRowUI>().Set(cost);
        }
    }

    private void UpdateConfirmButton()
    {
        if (currentRecipe == null)
        {
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable = PlayerStats.Instance.HasResources(currentRecipe);
    }

    public void ConfirmCraft()
    {
        if (currentRecipe == null)
        {
            return;
        }

        if (!PlayerStats.Instance.HasResources(currentRecipe))
        {
            return;
        }

        // Consume resources
        PlayerStats.Instance.ConsumeResources(currentRecipe);

        // Add item to inventory
        PlayerStats.Instance.GiveItem(currentRecipe);

        // Refresh resource list UI
        PopulateResourceList(currentRecipe);

        // UI refresh
        UpdateConfirmButton();
    }

    private void ClearDetails()
    {
        itemNameText.text = "";
        itemIcon.enabled = false;
        confirmButton.interactable = false;
    }
}
