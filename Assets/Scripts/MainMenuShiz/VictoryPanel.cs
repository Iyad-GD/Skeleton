using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


/// Handles updating the remaining lives display and provides clean, modular callbacks for win buttons.
public class VictoryPanel : MonoBehaviour
{
    [Header("Level Progress Settings")]
    [Tooltip("The index of the level currently being completed (e.g., 1 for Level 1). " +
             "If set, this will automatically register the completion and unlock the next level when this panel is shown.")]
    [SerializeField] private int currentLevelIndex = 1;

    [Tooltip("The exact name of the next level scene. If left empty, it will load the next scene in the Build Settings sequence (Active Scene + 1).")]
    [SerializeField] private string nextLevelSceneName = "";

    [Tooltip("The scene name for the Main Menu.")]
    [SerializeField] private string mainMenuSceneName = "Main Menu Trial";

    [Header("UI Visuals")]
    [Tooltip("The TextMeshPro text component where remaining lives will be displayed.")]
    [SerializeField] private TMP_Text livesCountText;

    [Tooltip("Prefix string before the lives count (e.g. 'Lives Remaining: ')")]
    [SerializeField] private string livesPrefix = "Lives Remaining: ";

    private void OnEnable()
    {
        PlayerMovement.IsMovementLocked = true;

        if (currentLevelIndex > 0)
        {
            LevelManager.CompleteLevel(currentLevelIndex);
        }

        
        DisplayRemainingLives();
    }

    private void OnDisable()
    {
        PlayerMovement.IsMovementLocked = false; // Unlock player when panel closes
    }


    public void DisplayRemainingLives()
    {
        if (livesCountText == null) return;

       
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                livesCountText.text = livesPrefix + playerHealth.CurrentLives.ToString();
            }
            else
            {
                Debug.LogWarning("[VictoryPanel] Player found, but PlayerHealth component is missing!");
                livesCountText.text = livesPrefix + "N/A";
            }
        }
        else
        {
            Debug.LogWarning("[VictoryPanel] Player GameObject not found in the scene.");
            livesCountText.text = livesPrefix + "?";
        }
    }


    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            Debug.Log($"[VictoryPanel] Loading Next Level: {nextLevelSceneName}");
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"[VictoryPanel] Loading Next Build Index Level: {nextSceneIndex}");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogError("[VictoryPanel] No more levels found in Build Settings! Please assign nextLevelSceneName manually.");
            }
        }
    }


    public void GoToMainMenu()
    {
        Debug.Log($"[VictoryPanel] Loading Main Menu: {mainMenuSceneName}");
        SceneManager.LoadScene(mainMenuSceneName);
    }


    public void QuitGame()
    {
        Debug.Log("[VictoryPanel] Quitting Application...");
        Application.Quit();
    }
}
