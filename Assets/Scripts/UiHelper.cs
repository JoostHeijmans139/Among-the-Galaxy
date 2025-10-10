using System;
using System.Collections;
using System.Collections.Generic;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiHelper: MonoBehaviour
{ 
    public TMP_InputField levelOfDetailInput;
    MapGenerator mapGenerator = MapGenerator.Instance;

    private void Start()
    {
        if (mapGenerator == null)
        {
            Debug.Log("Singleton instance is null, finding MapGenerator in scene.");
        }
        levelOfDetailInput.onEndEdit.AddListener(OnInputEnd);
    }

    public void GenerateMap()
    {
        SceneManager.LoadScene("WorldCreation");
    }
    void OnInputEnd(string inputText)
    {
        int LevelOfDetail = int.Parse(inputText);
        if (levelOfDetailInput!)
        {
            SetLevelOfDetail(LevelOfDetail);
        }
    }
    public void SetLevelOfDetail(float value)
    {
        if(value <0 || value >6)
        {
            Debug.LogWarning("Level of detail must be between 0 and 6.");
            return;
        }
        SettingsManager.CurrentSettings.levelOfDetail = (int)value;
        SettingsManager.SaveSettings();
    }
}
