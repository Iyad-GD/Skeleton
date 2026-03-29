using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("How much damage to deal per hit.")]
    public int damage = 1;

    [Tooltip("If true, kills the player instantly regardless of health.")]
    public bool instakill = false;

    [Tooltip("Seconds before this object can damage the player again. 0 = damages every frame.")]
    public float damageCooldown = 1f;

    [Header("References")]
    [Tooltip("Tag the player GameObject must have.")]
    public string playerTag = "Player";

    private float _cooldownTimer = 0f;

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Keeps dealing damage while player stands on it
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (_cooldownTimer > 0f) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogWarning("[DamageObject] Player has no PlayerHealth component!");
            return;
        }

        if (instakill)
            health.Die();
        else
            health.TakeDamage(damage);

        _cooldownTimer = damageCooldown;
    }
}
