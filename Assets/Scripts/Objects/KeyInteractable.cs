using UnityEngine;

public class KeyInteractable : MonoBehaviour
{
    public static bool HasKey { get; private set; } = false;

    [Header("Settings")]
    [Tooltip("Tag that the player GameObject must have.")]
    public string playerTag = "Player";

    [Tooltip("Key the player must press to pick up the object.")]
    public KeyCode pickupKey = KeyCode.E;

    [Header("Optional UI")]
    [Tooltip("Optional: a UI prompt to show when player is nearby.")]
    public GameObject promptUI;

    private bool _playerNearby = false;

    private void Start()
    {
        // Reset key state at scene start
        HasKey = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"[KeyInteractable] No Collider found on '{gameObject.name}'. " +
                             "Add a Collider and enable 'Is Trigger'.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"[KeyInteractable] Collider on '{gameObject.name}' is not set to Trigger. " +
                             "Enable 'Is Trigger' in the Inspector.");
        }

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerNearby && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerNearby = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerNearby = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    private void PickUp()
    {
        HasKey = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        Debug.Log("[KeyInteractable] Key picked up! You can now use it at the COMPUTA.");

        // Hide the key object
        gameObject.SetActive(false);
    }

    // Beautiful retro OnGUI fallback if custom UI prompt is not set up
    private void OnGUI()
    {
        if (_playerNearby && promptUI == null)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 24;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            GUIStyle shadowStyle = new GUIStyle(style);
            shadowStyle.normal.textColor = Color.black;

            string message = $"Press [{pickupKey}] to Pickup";

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

    public static void ResetKey()
    {
        HasKey = false;
    }
}
