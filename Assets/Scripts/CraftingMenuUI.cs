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
        confirmButton.interactable = PlayerStats.Instance.HasResources(currentRecipe);
    }

    public void ConfirmCraft()
    {
        if (!PlayerStats.Instance.HasResources(currentRecipe))
            return;

        PlayerStats.Instance.ConsumeResources(currentRecipe);
        PlayerStats.Instance.GiveItem(currentRecipe);

        UpdateConfirmButton();
    }

    private void ClearDetails()
    {
        itemNameText.text = "";
        itemIcon.enabled = false;
        confirmButton.interactable = false;
    }
}
