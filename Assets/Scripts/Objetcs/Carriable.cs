using UnityEngine;

/// <summary>
/// Attach to any object the player can pick up and throw.
/// Works alongside CarrySystem on the player.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Carriable : MonoBehaviour
{
    [Header("Settings")]
    public bool canBePickedUp = true;

    [Tooltip("Optional: plays when this object hits something after being thrown.")]
    public AudioClip impactSound;

    [Tooltip("Minimum velocity on impact to play sound / trigger hit logic.")]
    public float impactThreshold = 3f;

    private Rigidbody2D _rb;
    private Collider2D _col;
    private AudioSource _audio;
    private bool _wasThrown = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();

        if (impactSound != null)
        {
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
        }
    }

    public void OnPickedUp()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.velocity = Vector2.zero;
        _col.enabled = false;   // prevent clipping through things while carried
        _wasThrown = false;
        canBePickedUp = false;
    }

    public void OnDropped(Vector2 throwVelocity)
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _col.enabled = true;
        canBePickedUp = true;

        if (throwVelocity != Vector2.zero)
        {
            _rb.velocity = throwVelocity;
            _wasThrown = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!_wasThrown) return;

        float speed = col.relativeVelocity.magnitude;
        if (speed >= impactThreshold)
        {
            if (_audio != null && impactSound != null)
                _audio.Play();

            Debug.Log($"[Carriable] '{gameObject.name}' hit '{col.gameObject.name}' at speed {speed:F1}");

            // You can hook into other scripts here, e.g.:
            // col.gameObject.GetComponent<BreakableTile>()?.Hit();
        }

        _wasThrown = false;
    }
}
