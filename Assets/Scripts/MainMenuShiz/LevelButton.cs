using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Attached to level selection button GameObjects.
/// Automatically handles interactability, locked/completed visual overlays, and loading scenes.
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Button))]
public class LevelButton : MonoBehaviour
{
    [Header("Level Configuration")]
    [Tooltip("The 1-based index of this level (e.g. 1 for Level 1, 2 for Level 2)")]
    [SerializeField] private int levelIndex = 1;

    [Tooltip("The exact name of the Scene to load. If left empty, it will default to 'Level ' + levelIndex")]
    [SerializeField] private string sceneName = "";

    [Header("UI Visual References")]
    [Tooltip("Optional TextMeshPro component to show the level index/number")]
    [SerializeField] private TMP_Text levelText;

    [Tooltip("Optional overlay GameObject shown only when this level is locked")]
    [SerializeField] private GameObject lockVisual;

    [Tooltip("Optional overlay GameObject shown only when this level is completed")]
    [SerializeField] private GameObject completedVisual;

    private UnityEngine.UI.Button button;

    private void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        
        // Dynamically wire the click listener
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        RefreshVisuals();
    }

    /// <summary>
    /// Updates the button's interactable state and overlays based on level unlock and complete progress.
    /// </summary>
    public void RefreshVisuals()
    {
        bool isUnlocked = LevelManager.IsLevelUnlocked(levelIndex);
        bool isCompleted = LevelManager.IsLevelCompleted(levelIndex);

        // Set button interactability
        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        // Toggle the lock visual representation
        if (lockVisual != null)
        {
            lockVisual.SetActive(!isUnlocked);
        }

        // Toggle the completion checkmark or stars
        if (completedVisual != null)
        {
            completedVisual.SetActive(isCompleted);
        }

        // Configure the button text to match the level index
        if (levelText != null)
        {
            levelText.text = levelIndex.ToString();
        }
    }

    private void OnButtonClick()
    {
        // Safety check before loading scene
        if (LevelManager.IsLevelUnlocked(levelIndex))
        {
            string targetScene = string.IsNullOrEmpty(sceneName) ? "Level " + levelIndex : sceneName;
            
            Debug.Log($"[LevelButton] Loading Scene: {targetScene}");
            SceneManager.LoadScene(targetScene);
        }
    }

    // --- Editor Helper Context Menus ---
    // Right-click on the component in the Inspector to access these utilities.

    [ContextMenu("Unlock This Level")]
    private void EditorUnlockLevel()
    {
        LevelManager.UnlockLevel(levelIndex);
        RefreshVisuals();
    }

    [ContextMenu("Reset All Progress")]
    private void EditorResetProgress()
    {
        LevelManager.ResetProgress();
        RefreshVisuals();
    }
}
