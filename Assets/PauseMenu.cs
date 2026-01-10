using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused;
    public AudioSource backGroundAudioSource;
    public Slider volumeSlider;
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(isPaused);
            if (!isPaused)
            {
                isPaused = true;
                pauseMenuUI.SetActive(true);
                volumeSlider.value = backGroundAudioSource.volume;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
                pauseMenuUI.SetActive(false);
            }
        }
    }
}
