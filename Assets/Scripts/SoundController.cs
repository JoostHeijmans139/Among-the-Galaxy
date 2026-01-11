using Settings;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource _backGroundAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _backGroundAudioSource = GetComponent<AudioSource>();
        _backGroundAudioSource.playOnAwake = true;
        SettingsManager.LoadSettings();
        _backGroundAudioSource.volume = SettingsManager.CurrentSettings.masterVolume;
    }
    public void SetVolume(float volume)
    {
        _backGroundAudioSource.volume = volume;
        Debug.Log("Set background sound to volume level: "+volume);
    }
    
}
