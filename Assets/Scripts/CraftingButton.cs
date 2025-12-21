using UnityEngine;
using UnityEngine.UI;

// Script for each recipe that each Button has in crafting button list
public class CraftingButton : MonoBehaviour
{
    public CraftingRecipe recipe;
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        CraftingMenuUI.Instance.SelectRecipe(recipe);
    }
}
