using UnityEngine;

/// <summary>
/// Attach to the Player alongside PlayerMovement and CharacterController2D.
/// </summary>
public class WallJump : MonoBehaviour
{
    [Header("Wall Detection")]
    public LayerMask wallLayer;
    [Tooltip("How far to raycast sideways to detect a wall.")]
    public float wallCheckDistance = 0.35f;
    [Tooltip("Create an empty child GameObject at the player's mid-side and assign it here.")]
    public Transform wallCheck;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 1.5f;

    [Header("Wall Jump")]
    [Tooltip("Horizontal impulse away from the wall.")]
    public float wallJumpForceX = 8f;
    [Tooltip("Vertical impulse of the wall jump.")]
    public float wallJumpForceY = 14f;
    [Tooltip("Seconds horizontal input is ignored after wall jumping.")]
    public float wallJumpControlLockout = 0.2f;

    public bool IsWallSliding { get; private set; }
    public bool ControlLocked => _lockoutTimer > 0f;

    private CharacterController2D _controller;
    private Rigidbody2D _rb;
    private float _lockoutTimer = 0f;
    private bool _touchingRight;
    private bool _touchingLeft;

    private void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_lockoutTimer > 0f)
            _lockoutTimer -= Time.deltaTime;

        CheckWalls();
        HandleWallSlide();
    }

    private void CheckWalls()
    {
        Vector2 origin = wallCheck != null ? (Vector2)wallCheck.position : (Vector2)transform.position;
        _touchingRight = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);
        _touchingLeft  = Physics2D.Raycast(origin, Vector2.left,  wallCheckDistance, wallLayer);
    }

    private void HandleWallSlide()
    {
        bool onWall = _touchingRight || _touchingLeft;
        IsWallSliding = onWall && !_controller.IsGrounded && _rb.velocity.y < 0f;

        if (IsWallSliding)
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -wallSlideSpeed));
    }

    // Called by PlayerMovement when jump is pressed during a wall slide
    public void TriggerWallJump()
    {
        // Push away from whichever wall we're on
        float dirX = _touchingRight ? -1f : 1f;
        Vector2 force = new Vector2(dirX * wallJumpForceX, wallJumpForceY);

        _controller.ApplyWallJumpForce(force);
        _lockoutTimer = wallJumpControlLockout;
        IsWallSliding = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheck == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallCheck.position, Vector2.right * wallCheckDistance);
        Gizmos.DrawRay(wallCheck.position, Vector2.left  * wallCheckDistance);
    }
}
