using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Starting health of player
    public float Health { get; set; } = 100f;

    // Player receives damage function
    public void TakeDamage(float damage)
    {
        Health = Health - damage;
    }


    // Quick test for damage to UI
    [ContextMenu("Debug Damage (10)")]
    private void DebugTakeDamage()
    {
        TakeDamage(10f);
        Debug.Log("-10 damage, Health = " + Health);
    }
}
