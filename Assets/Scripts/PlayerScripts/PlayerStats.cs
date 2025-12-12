using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance { get; private set; }

    // Starting health of player
    public float Health = 100f;

    // Crafting resources
    public int Wood { get; set; } = 0;
    public int Stone { get; set; } = 0;
    public int Metal { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int Slime { get; set; } = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Player receives damage function
    public void TakeDamage(float damage)
    {
        Health = Health - damage;
    }

    // Player gaining resources function
    public void GainResource(string type, int amount)
    {
        switch (type.FirstCharacterToUpper())
        {
            case "Wood":
                Wood = Wood + amount;
                break;

            case "Stone":
                Stone = Stone + amount;
                break;

            case "Metal":
                Metal = Metal + amount;
                break;

            case "Gold":
                Gold = Gold + amount;
                break;

            case "Slime":
                Slime = Slime + amount;
                break;
        }
    }


    // Quick test for damage to UI
    [ContextMenu("Debug Damage (10)")]
    private void DebugTakeDamage()
    {
        TakeDamage(10f);
        Debug.Log("-10 damage, Health = " + Health);
    }

    [ContextMenu("Debug Give Wood (1)")]
    private void DebugGiveWood()
    {
        GainResource("Wood", 1);
        Debug.Log("+1 wood gained, Wood = " + Wood);
    }
}
