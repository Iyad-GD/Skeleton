using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Death Behaviour")]
    [Tooltip("Respawn the player at a set point instead of reloading the scene.")]
    public bool useRespawnPoint = false;
    public Transform respawnPoint;

    [Header("Invincibility Frames")]
    [Tooltip("Seconds of invincibility after taking damage (prevents rapid multi-hits).")]
    public float invincibilityDuration = 0.5f;

    [Header("Visual Feedback (optional)")]
    [Tooltip("SpriteRenderer to flash when hurt.")]
    public SpriteRenderer spriteRenderer;

    public int CurrentHealth { get; private set; }
    private bool _isInvincible = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible) return;

        CurrentHealth -= amount;
        Debug.Log($"[PlayerHealth] Took {amount} damage. Health: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    public void Die()
    {
        Debug.Log("[PlayerHealth] Player died.");
        CurrentHealth = 0;

        StopAllCoroutines();

        if (useRespawnPoint && respawnPoint != null)
        {
            Respawn();
        }
        else
        {
            // Reload current scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        CurrentHealth = maxHealth;
        _isInvincible = false;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        Debug.Log("[PlayerHealth] Player respawned.");
    }

    private IEnumerator InvincibilityRoutine()
    {
        _isInvincible = true;

        // Flash the sprite while invincible
        if (spriteRenderer != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.5f); // red tint
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.2f;
            }
            spriteRenderer.color = Color.white;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }

        _isInvincible = false;
    }

}
