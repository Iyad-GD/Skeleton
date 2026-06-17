using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
        Debug.Log("WORKING");
    }

    public void Play()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
    }
    public void GoToScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Settings()
    {

    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application Quit UWU");
    }

}
