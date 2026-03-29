using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The SlidingDoor GameObject. ")]
    public SlidingDoor linkedDoor;

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
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"[KeyPickup] No Collider found on '{gameObject.name}'. " +
                             "Add a Collider and enable 'Is Trigger'.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"[KeyPickup] Collider on '{gameObject.name}' is not set to Trigger. " +
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
        // Hide prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // Tell door to open
        if (linkedDoor != null)
        {
            linkedDoor.Open();
        }
        else
        {
            Debug.LogWarning("[KeyPickup] No SlidingDoor linked! Assign one in the Inspector.");
        }

        Destroy(gameObject);
    }

}
