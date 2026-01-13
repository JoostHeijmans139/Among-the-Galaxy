using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;

    // Type of weapon
    public PlayerStats.weapon weaponType;

    public List<ResourceCost> costs;

    // Weapon being upgraded
    public PlayerStats.weapon requiredWeapon = PlayerStats.weapon.None;
}

[System.Serializable]
public struct ResourceCost
{
    public string resourceName;
    public int amount;
}