using System.Collections;
using UnityEngine;

/// purely decorative character in the main menu: it constantly "walks"
/// (keeping the Animator's Speed parameter positive) and periodically performs a
/// jump (IsJumping) with a real vertical hop. No physics, input or death logic.
/// The world is expected to scroll past it (treadmill), so it stays in place.

public class MenuSlime : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Walking")]
    [Tooltip("Value written to the 'Speed' animator parameter to keep the walk animation playing.")]
    public float walkAnimSpeed = 10f;

    [Header("Jumping")]
    public float minJumpInterval = 2.5f;
    public float maxJumpInterval = 5f;
    [Tooltip("Peak height of the hop in world units.")]
    public float jumpHeight = 18f;
    [Tooltip("Duration of a full hop in seconds.")]
    public float jumpDuration = 0.6f;

    private float _baselineY;
    private float _timer;
    private float _nextJump;
    private bool _jumping;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        _baselineY = transform.position.y;
        _nextJump = Random.Range(minJumpInterval, maxJumpInterval);
    }

    void Update()
    {
        if (animator != null)
            animator.SetFloat("Speed", walkAnimSpeed);

        if (_jumping) return;

        _timer += Time.deltaTime;
        if (_timer >= _nextJump)
        {
            _timer = 0f;
            _nextJump = Random.Range(minJumpInterval, maxJumpInterval);
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        _jumping = true;
        _baselineY = transform.position.y;
        if (animator != null) animator.SetBool("IsJumping", true);

        float t = 0f;
        while (t < jumpDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / jumpDuration);
            float h = 4f * jumpHeight * n * (1f - n); // parabolic arc: 0 -> peak -> 0
            Vector3 p = transform.position;
            p.y = _baselineY + h;
            transform.position = p;
            yield return null;
        }

        Vector3 end = transform.position;
        end.y = _baselineY;
        transform.position = end;

        if (animator != null) animator.SetBool("IsJumping", false);
        _jumping = false;
    }
}
