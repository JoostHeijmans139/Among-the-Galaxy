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
    public List<GameObject> MenuParents;
    public void GenerateMap()
    {
        SceneManager.LoadScene("WorldCreation");
    }
    public void SetLevelOfDetail(float value)
    {
        SettingsManager.CurrentSettings.levelOfDetail = (int)value;
        Debug.Log("Level of Detail set to: " + value);
        SettingsManager.SaveSettings();
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
