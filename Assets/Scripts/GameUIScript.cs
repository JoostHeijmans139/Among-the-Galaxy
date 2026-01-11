using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public TextMeshProUGUI healthTextbox;
    public TextMeshProUGUI woodTextbox;
    public TextMeshProUGUI stoneTextbox;
    public TextMeshProUGUI metalTextbox;
    public TextMeshProUGUI goldTextbox;
    public TextMeshProUGUI slimeTextbox;
    public PlayerStats playerStats;

    private void Start()
    {
        // Reconnect to PlayerStats singleton if reference is lost
        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
        }
    }
    
    void Update()
    {
        // Reconnect to PlayerStats singleton if reference is lost during runtime
        if (playerStats == null && PlayerStats.Instance != null)
        {
            playerStats = PlayerStats.Instance;
        }

        // Show Health if these elements arent filled
        if (playerStats == null || healthTextbox == null || woodTextbox == null || stoneTextbox == null || metalTextbox == null || goldTextbox == null || slimeTextbox == null)
        {
            //Error
            Debug.LogError("ERROR: either playerStats doesn't exsist or one of the many UI elements");
        }
        else
        {
            healthTextbox.text = "Health: " + playerStats.Health.ToString("F0");
            woodTextbox.text = "W: " + playerStats.Wood.ToString();
            stoneTextbox.text = "S: " + playerStats.Stone.ToString();
            metalTextbox.text = "M: " + playerStats.Metal.ToString();
            goldTextbox.text = "G: " + playerStats.Gold.ToString();
            slimeTextbox.text = "S: " + playerStats.Slime.ToString();
        }

    }
}
