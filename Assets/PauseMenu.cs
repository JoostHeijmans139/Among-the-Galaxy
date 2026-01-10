using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused;
    AudioSource _backGroundAudioSource;
    Slider _volumeSlider;
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(isPaused);
            if (!isPaused)
            {
                isPaused = true;
                pauseMenuUI.SetActive(true);
                _volumeSlider.value = _backGroundAudioSource.volume;
            }
            else
            {
                isPaused = false;
                pauseMenuUI.SetActive(false);
            }
        }
    }
}
