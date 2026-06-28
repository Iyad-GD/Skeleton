using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject levelSelectionPanel;

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic("Main Menu");
        }

        // Ensure initial panel visibility states
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
    }

    public void Play()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic("Main Menu");
        }
    }

    public void GoToScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Settings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
    }

    public void OpenLevelSelection()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application Quit UWU");
    }
}
