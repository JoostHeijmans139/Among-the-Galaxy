using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public TextMeshProUGUI textbox;
    public PlayerStats playerStats;
    
    void Update()
    {
        // Show Health if these elements arent filled
        if (playerStats != null & textbox != null)
        {
            textbox.text = "Health: " + playerStats.Health.ToString("F0");
        }
    }
}
