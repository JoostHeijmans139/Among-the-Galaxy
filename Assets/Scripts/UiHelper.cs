using System;
using System.Collections;
using System.Collections.Generic;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UiHelper: MonoBehaviour
{ 
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
}
