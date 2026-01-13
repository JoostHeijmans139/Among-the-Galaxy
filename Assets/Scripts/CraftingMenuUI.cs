using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Code for generating the results area of a crafting recipe
public class CraftingMenuUI : MonoBehaviour
{
    public static CraftingMenuUI Instance;

    [Header("Details UI")]
    public TMP_Text itemNameText;
    public TMP_Text weaponRequirementText;
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

        // Weapon requirement text update
        // If player already made a better weapon
        if ((int)currentRecipe.weaponType < (int)PlayerStats.Instance.equippedWeapon)
        {
            weaponRequirementText.text = "Cannot craft a worse axe than your current weapon";
            weaponRequirementText.gameObject.SetActive(true);
        }
        // If player already has this weapon
        else if (currentRecipe.weaponType == PlayerStats.Instance.equippedWeapon)
        {
            weaponRequirementText.text = "Cannot craft currently equipped item";
            weaponRequirementText.gameObject.SetActive(true);
        }
        // If player doesn't have the required weapon
        else if (currentRecipe.requiredWeapon != PlayerStats.Instance.equippedWeapon)
        {
            weaponRequirementText.text = "Requires previous weapon";
            weaponRequirementText.gameObject.SetActive(true);
        }
        else
        {
            weaponRequirementText.gameObject.SetActive(false);
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

        // Make player equip new weapon
        PlayerStats.Instance.equippedWeapon = currentRecipe.weaponType;

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
