
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Singleton instance
    public static PlayerStats Instance { get; private set; }

    public PlayerAttackRange attackRange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Starting health of player
    public float Health = 100f;

    private EnemyAI enemyInRange;

    // Crafting resources
    public int Wood { get; set; } = 0;
    public int Stone { get; set; } = 0;
    public int Metal { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Slime { get; set; } = 0;

    // Player inventory
    public List<string> Inventory = new List<string>();

    // Player gaining resources function
    public void GainResource(string type, int amount)
    {
        switch (type.FirstCharacterToUpper())
        {
            case "Wood": 
                Wood += amount; 
                break;

            case "Stone": 
                Stone += amount; 
                break;

            case "Metal": 
                Metal += amount; 
                break;

            case "Gold": 
                Gold += amount; 
                break;

            case "Slime": 
                Slime += amount; 
                break;
        }
    }

    public void LoseResource(string type, int amount)
    {
        switch (type.FirstCharacterToUpper())
        {
            case "Wood": 
                Wood = Mathf.Max(0, Wood - amount); 
                break;

            case "Stone": 
                Stone = Mathf.Max(0, Stone - amount); 
                break;

            case "Metal": 
                Metal = Mathf.Max(0, Metal - amount); 
                break;

            case "Gold": 
                Gold = Mathf.Max(0, Gold - amount); 
                break;

            case "Slime": 
                Slime = Mathf.Max(0, Slime - amount); 
                break;
        }
    }

    public int GetAmount(string type)
    {
        switch (type.FirstCharacterToUpper())
        {
            case "Wood": 
                return Wood;

            case "Stone": 
                return Stone;

            case "Metal": 
                return Metal;

            case "Gold": 
                return Gold;

            case "Slime": 
                return Slime;

            default:
                Debug.LogWarning("Resource type not found: " + type);
                return 0;
        }
    }

    // Check if player has enough resources for recipe
    public bool HasResources(CraftingRecipe recipe)
    {
        foreach (var cost in recipe.costs)
        {
            if (GetAmount(cost.resourceName) < cost.amount)
            {
                // Player does NOT have enough
                return false;
            }
        }
        // Player has enough
        return true;
    }

    // Remove resources based on a recipe
    public void ConsumeResources(CraftingRecipe recipe)
    {
        foreach (var cost in recipe.costs)
        {
            LoseResource(cost.resourceName, cost.amount);
        }
    }

    // Add crafted item to player's inventory
    public void GiveItem(CraftingRecipe recipe)
    {
        Inventory.Add(recipe.itemName);
    }

    //Debug
    [ContextMenu("Debug Give Wood (1)")]
    private void DebugGiveWood()
    {
        GainResource("Wood", 1);
        Debug.Log("+1 wood gained, Wood = " + Wood);
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && attackRange.enemyInRange != null)
        {
            attackRange.enemyInRange.TakeDamage(10f);
        }
    }

    private void Update()
    {
        Attack();
    }
}

// Extension method to capitalize first letter
public static class StringExtensions
{
    public static string FirstCharacterToUpper(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }
}
