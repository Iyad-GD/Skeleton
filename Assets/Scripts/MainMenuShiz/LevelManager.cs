using UnityEngine;


public static class LevelManager
{
    private const string UNLOCKED_KEY_PREFIX = "Level_Unlocked_";
    private const string COMPLETED_KEY_PREFIX = "Level_Completed_";
    
    // By default, the first levels are unlocked
    private const int DEFAULT_UNLOCKED_COUNT = 1;

    
    /// Checks if a level is unlocked. Level index is 1-based (e.g. Level 1 is index 1).
    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex <= 0) return false;

       
        if (levelIndex <= DEFAULT_UNLOCKED_COUNT)
        {
            return true;
        }

        // For other levels, check PlayerPrefs (1 = unlocked, 0 = locked)
        return PlayerPrefs.GetInt(UNLOCKED_KEY_PREFIX + levelIndex, 0) == 1;
    }

    /// Unlocks a specific level index.
    /// Call this modularly from any finished goal script or trigger.
    public static void UnlockLevel(int levelIndex)
    {
        if (levelIndex <= 0) return;

        PlayerPrefs.SetInt(UNLOCKED_KEY_PREFIX + levelIndex, 1);
        PlayerPrefs.Save();
        Debug.Log($"[LevelManager] Level {levelIndex} is now unlocked!");
    }

    /// Completes the current level and automatically unlocks the next level.
    /// Call this when the player finishes/completes a level.
    public static void CompleteLevel(int levelIndex)
    {
        if (levelIndex <= 0) return;

        PlayerPrefs.SetInt(COMPLETED_KEY_PREFIX + levelIndex, 1);
        UnlockLevel(levelIndex + 1);
        PlayerPrefs.Save();
        Debug.Log($"[LevelManager] Level {levelIndex} completed! Level {levelIndex + 1} is now unlocked.");
    }

    /// Checks if a level has been completed.
    public static bool IsLevelCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt(COMPLETED_KEY_PREFIX + levelIndex, 0) == 1;
    }

    /// Resets all level unlock and completion progress back to default.
    public static void ResetProgress(int maxLevelsToReset = 100)
    {
        for (int i = 1; i <= maxLevelsToReset; i++)
        {
            PlayerPrefs.DeleteKey(UNLOCKED_KEY_PREFIX + i);
            PlayerPrefs.DeleteKey(COMPLETED_KEY_PREFIX + i);
        }
        PlayerPrefs.Save();
        Debug.Log("[LevelManager] All level progress reset. Levels 1 unlocked by default.");
    }
}
