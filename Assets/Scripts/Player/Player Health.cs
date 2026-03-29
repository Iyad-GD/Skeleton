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

    [Header("Death Body")]
    [Tooltip("Prefab to spawn as a corpse when the player dies. Should have a Rigidbody2D so it inherits momentum.")]
    public GameObject deathBodyPrefab;
    [Tooltip("How much of the player's velocity the body inherits on death.")]
    [Range(0f, 2f)]
    public float bodyMomentumMultiplier = 1f;

    [Header("Invincibility Frames")]
    [Tooltip("Seconds of invincibility after taking damage (prevents rapid multi-hits).")]
    public float invincibilityDuration = 0.5f;

    [Header("Visual Feedback (optional)")]
    [Tooltip("SpriteRenderer to flash when hurt. Leave empty to skip.")]
    public SpriteRenderer spriteRenderer;

    public int CurrentHealth { get; private set; }
    private bool _isInvincible = false;
    private Rigidbody2D _rb;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible) return;

        CurrentHealth -= amount;
        Debug.Log($"[PlayerHealth] Took {amount} damage. Health: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityRoutine());
    }

    public void Die()
    {
        Debug.Log("[PlayerHealth] Player died.");
        CurrentHealth = 0;

        StopAllCoroutines();

        SpawnDeathBody();

        if (useRespawnPoint && respawnPoint != null)
            Respawn();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void SpawnDeathBody()
    {
        if (deathBodyPrefab == null) return;

        GameObject body = Instantiate(deathBodyPrefab, transform.position, transform.rotation);

        // Pass velocity to the body
        if (_rb != null)
        {
            Rigidbody2D bodyRb = body.GetComponent<Rigidbody2D>();
            if (bodyRb != null)
                bodyRb.velocity = _rb.velocity * bodyMomentumMultiplier;
        }

        // UNCOMMENT IF USING SCENE RELOAD ONLY so it survives if you want it to persist
        // DontDestroyOnLoad(body);
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

        if (spriteRenderer != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.5f);
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
