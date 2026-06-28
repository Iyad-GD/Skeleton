using UnityEngine;

public class ComputerInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The ArrowMinigame manager in the canvas.")]
    public ArrowMinigame minigame;

    [Tooltip("The SlidingDoor GameObject that this terminal unlocks.")]
    public SlidingDoor linkedDoor;

    [Header("Settings")]
    [Tooltip("Tag that the player GameObject must have.")]
    public string playerTag = "Player";

    [Tooltip("Key the player must press to interact.")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Optional UI Prompts")]
    [Tooltip("Activated when player is nearby but DOES NOT have the key.")]
    public GameObject promptKeyNeeded;

    [Tooltip("Activated when player is nearby and DOES have the key.")]
    public GameObject promptKeyReady;

    private bool _playerNearby = false;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"[ComputaController] No Collider found on '{gameObject.name}'. " +
                             "Add a Collider and enable 'Is Trigger'.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"[ComputaController] Collider on '{gameObject.name}' is not set to Trigger. " +
                             "Enable 'Is Trigger' in the Inspector.");
        }

        UpdatePrompts();
    }

    private void Update()
    {
        if (_playerNearby)
        {
            // Update prompts dynamically in case key was just acquired
            UpdatePrompts();

            if (Input.GetKeyDown(interactKey))
            {
                if (KeyInteractable.HasKey)
                {
                    if (minigame != null)
                    {
                        // Hide prompts during minigame
                        HidePrompts();
                        minigame.StartMinigame(linkedDoor);
                    }
                    else
                    {
                        Debug.LogError("[ComputaController] ArrowMinigame is not assigned in the inspector!");
                    }
                }
                else
                {
                    Debug.Log("[ComputaController] Cannot access. Key is needed!");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerNearby = true;
            UpdatePrompts();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerNearby = false;
            HidePrompts();
        }
    }

    private void UpdatePrompts()
    {
        if (!_playerNearby)
        {
            HidePrompts();
            return;
        }

        bool hasKey = KeyInteractable.HasKey;

        if (promptKeyNeeded != null)
            promptKeyNeeded.SetActive(!hasKey);

        if (promptKeyReady != null)
            promptKeyReady.SetActive(hasKey);
    }

    private void HidePrompts()
    {
        if (promptKeyNeeded != null)
            promptKeyNeeded.SetActive(false);

        if (promptKeyReady != null)
            promptKeyReady.SetActive(false);
    }

    
    private void OnGUI()
    {
        if (_playerNearby && promptKeyNeeded == null && promptKeyReady == null)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 24;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            GUIStyle shadowStyle = new GUIStyle(style);
            shadowStyle.normal.textColor = Color.black;

            string message = KeyInteractable.HasKey
                ? $"Press [{interactKey}] to Interact"
                : "Key Needed!";

            float width = 500;
            float height = 50;
            float x = (Screen.width - width) / 2;
            float y = Screen.height - 150;

            // Draw Shadow
            GUI.Label(new Rect(x + 2, y + 2, width, height), message, shadowStyle);
            // Draw Main
            GUI.Label(new Rect(x, y, width, height), message, style);
        }
    }
}
