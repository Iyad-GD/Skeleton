using System.Collections;
using UnityEngine;


/// Setup: assign sprites for each crack stage in the Inspector
public class BreakableTile : MonoBehaviour
{
    [Header("Crack Stages")]
    [Tooltip("Sprites from intact to most cracked. Last sprite shows just before collapsing.")]
    public Sprite[] crackSprites;

    [Header("Timing")]
    [Tooltip("Seconds the player must stand on it before it advances one crack stage.")]
    public float timePerStage = 0.4f;

    [Tooltip("Seconds the tile stays 'broken' (invisible/disabled) before respawning. 0 = never respawns.")]
    public float respawnDelay = 3f;

    [Header("Feel")]
    [Tooltip("How far the tile shakes when cracking.")]
    public float shakeAmount = 0.05f;

    [Tooltip("Tag the player must have to trigger cracking.")]
    public string playerTag = "Player";

    private SpriteRenderer _sr;
    private Collider2D _col;
    private int _currentStage = 0;
    private bool _isBroken = false;
    private bool _playerOn = false;
    private float _stageTimer = 0f;
    private Vector3 _originPos;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _originPos = transform.position;

        if (crackSprites != null && crackSprites.Length > 0)
            _sr.sprite = crackSprites[0];
    }

    private void Update()
    {
        if (_isBroken || !_playerOn) return;

        _stageTimer += Time.deltaTime;

        if (_stageTimer >= timePerStage)
        {
            _stageTimer = 0f;
            AdvanceStage();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(playerTag)) return;

        // Only count as "on top" if player is above the tile
        if (IsPlayerAbove(col))
            _playerOn = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(playerTag)) return;
        _playerOn = false;
        // Tile does NOT reset — stays at current crack stage
    }

    private bool IsPlayerAbove(Collision2D col)
    {
        foreach (ContactPoint2D contact in col.contacts)
            if (contact.normal.y < -0.5f) return true;
        return false;
    }

    private void AdvanceStage()
    {
        _currentStage++;

        // Update sprite
        if (crackSprites != null && _currentStage < crackSprites.Length)
            _sr.sprite = crackSprites[_currentStage];

        // Shake
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(Shake());

        // Collapse on last stage
        if (crackSprites == null || _currentStage >= crackSprites.Length)
            StartCoroutine(Collapse());
    }

    private IEnumerator Collapse()
    {
        _isBroken = true;
        _playerOn = false;

        yield return new WaitForSeconds(0.1f); // tiny delay so player feels it go

        _col.enabled = false;
        _sr.enabled = false;

        if (respawnDelay > 0f)
        {
            yield return new WaitForSeconds(respawnDelay);
            Respawn();
        }
    }

    private void Respawn()
    {
        _currentStage = 0;
        _isBroken = false;
        _stageTimer = 0f;
        transform.position = _originPos;

        _col.enabled = true;
        _sr.enabled = true;

        if (crackSprites != null && crackSprites.Length > 0)
            _sr.sprite = crackSprites[0];
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            transform.position = _originPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _originPos;
    }
}
