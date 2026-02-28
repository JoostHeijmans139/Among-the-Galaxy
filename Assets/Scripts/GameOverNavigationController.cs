using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles controller navigation for the Game Over screen
/// </summary>
public class GameOverNavigationController : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Button restartButton;

    private EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found!");
        }
    }

    private void OnEnable()
    {
        // Wait one frame to ensure all UI elements are initialized
        StartCoroutine(SelectFirstButtonDelayed());
    }

    private IEnumerator SelectFirstButtonDelayed()
    {
        // Use realtime wait since Time.timeScale = 0 during game over
        yield return new WaitForSecondsRealtime(0.1f);
        
        SelectFirstButton();
    }

    /// <summary>
    /// Selects the restart button for controller navigation
    /// </summary>
    public void SelectFirstButton()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem is null!");
                return;
            }
        }

        // Select the restart button
        if (restartButton != null && restartButton.interactable && restartButton.gameObject.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(restartButton.gameObject);
            Debug.Log($"Selected restart button! Current selected: {eventSystem.currentSelectedGameObject?.name}");
        }
        else
        {
            Debug.LogWarning($"Restart button not selectable - Button null: {restartButton == null}, Interactable: {restartButton?.interactable}, Active: {restartButton?.gameObject.activeInHierarchy}");
        }
    }

    private void Update()
    {
        // Auto-reselect if selection is lost
        if (gameObject.activeInHierarchy && eventSystem != null && eventSystem.currentSelectedGameObject == null)
        {
            SelectFirstButton();
        }
    }
}
