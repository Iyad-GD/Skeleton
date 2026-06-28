using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitBehaviour: MonoBehaviour
{
    [Header("Panels")]
    public GameObject FinishGamePanel;
    [SerializeField] private int currentLevelIndex = 1;
    //[SerializeField] private string mainMenuSceneName = "Main Menu Trial";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Mark current level complete and unlock the next level
            LevelManager.CompleteLevel(currentLevelIndex);
            if (FinishGamePanel != null) FinishGamePanel.SetActive(true);

            // 2. Return to Main Menu or load the level selection screen
            //SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}