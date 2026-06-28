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

    [Header("Lives")]
    [Tooltip("Number of lives the player starts with.")]
    public int maxLives = 3;

    [Header("Game Over UI")]
    [Tooltip("Optional panel to show when game is over.")]
    public GameObject gameOverPanel;
    [Tooltip("Delay in seconds before reloading scene on game over.")]
    public float gameOverReloadDelay = 3f;

    [Header("Visual Feedback (optional)")]
    [Tooltip("SpriteRenderer to flash when hurt. Leave empty to skip.")]
    public SpriteRenderer spriteRenderer;

    public int CurrentHealth { get; private set; }
    public int CurrentLives { get; private set; }
    public event System.Action<int> OnLivesChanged;

    private bool _isInvincible = false;
    private Rigidbody2D _rb;
    private Animator _animator;
    private bool _isDead = false;
    private Coroutine _invincibilityCoroutine;

    private void Awake()
    {

        CurrentHealth = maxHealth;
        CurrentLives = maxLives;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (_isInvincible || _isDead) return;

        CurrentHealth -= amount;
        Debug.Log($"[PlayerHealth] Took {amount} damage. Health: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
            Die();
        else
        {
            if (_invincibilityCoroutine != null)
                StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
        }
    }

    public void Die()
    {
        if (_isDead) return;
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        _isDead = true;
        Debug.Log("[PlayerHealth] Player died.");
        CurrentHealth = 0;

        // Stop invincibility flashing
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = null;
        }

        // Cache velocity 
        Vector2 deathVelocity = Vector2.zero;
        if (_rb != null)
        {
            deathVelocity = _rb.velocity;
        }

        // Trigger the death animation
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }

        // Disable player movement and control
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Static;
        }

        // Disable collision to avoid getting hit further or blocking enemies
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Wait for the slime_die (duration is 1.2s)
        yield return new WaitForSeconds(1.2f);

        
        SpawnDeathBody(deathVelocity);

        CurrentLives--;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives > 0)
        {
            if (useRespawnPoint && respawnPoint != null)
                Respawn();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("[PlayerHealth] Game Over - Out of Lives.");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            yield return new WaitForSeconds(gameOverReloadDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void SpawnDeathBody(Vector2 velocity)
    {
        if (deathBodyPrefab == null) return;

        GameObject body = Instantiate(deathBodyPrefab, transform.position, transform.rotation);

        // Pass velocity
        Rigidbody2D bodyRb = body.GetComponent<Rigidbody2D>();
        if (bodyRb != null)
        {
            bodyRb.velocity = velocity * bodyMomentumMultiplier;
        }

        // UNCOMMENT IF USING SCENE RELOAD ONLY so it survives if you want it to persist
        // DontDestroyOnLoad(body);
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        CurrentHealth = maxHealth;
        _isInvincible = false;
        _isDead = false;

        // Re-enable player movement and control
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
        }

        // Restore Rigidbody settings
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.velocity = Vector2.zero;
        }

        // Re-enable collision
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        // Reset Animator states
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

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
