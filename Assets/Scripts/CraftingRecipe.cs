using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    public ResourceCost[] costs;
}

[System.Serializable]
public struct ResourceCost
{
    public string resourceName;
    public int amount;
}
