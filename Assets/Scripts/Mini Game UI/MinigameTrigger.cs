using UnityEngine;

/// <summary>
/// Attach to the world object the player walks up to (e.g. a terminal, lever, chest).
/// When the player presses E nearby, it opens the minigame.
/// </summary>
public class MinigameTrigger : MonoBehaviour
{
    [Header("References")]
    public ArrowMinigame minigame;
    public SlidingDoor linkedDoor;

    [Header("Settings")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("Optional")]
    public GameObject promptUI;

    private bool _playerNearby = false;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerNearby && Input.GetKeyDown(interactKey))
            minigame.StartMinigame(linkedDoor);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerNearby = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerNearby = false;
        if (promptUI != null) promptUI.SetActive(false);
    }
}
