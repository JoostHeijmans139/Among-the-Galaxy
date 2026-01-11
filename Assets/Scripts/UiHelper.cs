using System;
using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UiHelper: MonoBehaviour
{
    public AudioSource _MenuAudioSource;
    public List<GameObject> MenuParents;
    public void GenerateMap()
    {
        _MenuAudioSource.Stop();
        SceneManager.LoadScene("WorldCreation");
    }

    public void Start()
    {
        PlayMenuSound();
    }

    public void RestartGame()
    {
        // Reset player stats before restarting
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetStats();
        }
        
        // Unpause the game in case it was paused
        Time.timeScale = 1;
        
        // Reset cursor state to ensure player can move
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void PlayMenuSound()
    {
        _MenuAudioSource.Play();
    }
    public void LoadSettingsMenu()
    {
        foreach (GameObject menuItem in MenuParents)
        {
            switch (menuItem.tag)
            {
                case "MainMenu":
                    menuItem.SetActive(false);
                    break;
                case "SettingsMenu":
                    menuItem.SetActive(true);
                    break;
                default:
                    Debug.Log("No menu's found");
                    break;
            }
        }
    }
    public void LoadMainMenu()
    {
        foreach (GameObject menuItem in MenuParents)
        {
            switch (menuItem.tag)
            {
                case "MainMenu":
                    menuItem.SetActive(true);
                    break;
                case "SettingsMenu":
                    menuItem.SetActive(false);
                    break;
                default:
                    Debug.Log("No menu's found");
                    break;
                    
            }
        }
    }
}
