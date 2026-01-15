using System;
using System.Collections;
using System.Collections.Generic;
using Settings;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UiHelper: MonoBehaviour
{
    public AudioSource _MenuAudioSource;
    public List<GameObject> MenuParents;
    public static GameObject _gameOverscreen;
    public static TMPro.TMP_Text _timeText;
    public static GameObject _resourceDisplayPanel;
    public void GenerateMap()
    {
        _MenuAudioSource.Stop();
        SceneManager.LoadScene("WorldCreation");
    }

    public void Start()
    {
        if(_MenuAudioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
            _MenuAudioSource = GetComponent<AudioSource>();
        }
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("WorldCreation"))
        {
            _MenuAudioSource.enabled = false;
            _gameOverscreen = GameObject.FindGameObjectWithTag("GameOverScreen");
            if (_gameOverscreen != null)
            {
                Debug.Log("Successfully found GameOverScreen in scene.");
                _timeText = GameObject.FindGameObjectWithTag("TimeSurvivedText").GetComponent<TMPro.TMP_Text>();
                _resourceDisplayPanel = GameObject.FindGameObjectWithTag("ResourceDisplayPanel");
                if (_resourceDisplayPanel == null)
                {
                    Debug.Log("ResourceDisplayPanel not found in scene. Please ensure there is a UI panel with the 'ResourceDisplayPanel' tag.");
                    return;
                }
                if(_timeText == null)
                {
                    Debug.LogError("TimeSurvivedText not found in scene. Please ensure there is a TextMeshPro text object with the 'TimeSurvivedText' tag.");
                    return;
                }
                _gameOverscreen.SetActive(false);
            }
            else
            {
                Debug.Log("GameOverScreen not found in scene. Please ensure there is a UI panel with the 'GameOverScreen' tag.");
                return;
            }
        }
        PlayMenuSound();
        

    }
    
    public static void SetTimeSurvived(float time, TMPro.TMP_Text text)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string niceTime = string.Format( "You survived "+"{0:0} seconds : and {1:00} minutes",seconds,minutes);
        text.text = niceTime;
        _resourceDisplayPanel.SetActive(false);
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
