using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Format Settings")]
    [SerializeField] private string prefix = "Lives: ";

    private PlayerHealth _playerHealth;

    private void Start()
    {
        // Find player health in the scene
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerHealth = player.GetComponent<PlayerHealth>();
            if (_playerHealth != null)
            {
                _playerHealth.OnLivesChanged += UpdateLivesUI;
                // Initial update
                UpdateLivesUI(_playerHealth.CurrentLives);
            }
            else
            {
                Debug.LogError("[PlayerLivesUI] Player found but PlayerHealth component is missing!");
            }
        }
        else
        {
            Debug.LogError("[PlayerLivesUI] Player GameObject named 'Player' not found in scene!");
        }
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnLivesChanged -= UpdateLivesUI;
        }
    }

    private void UpdateLivesUI(int currentLives)
    {
        if (livesText != null)
        {
            livesText.text = prefix + currentLives.ToString();
        }
    }
}
