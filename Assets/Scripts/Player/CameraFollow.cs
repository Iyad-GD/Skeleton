using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothing")]
    [Tooltip("Higher = snappier. Lower = floatier.")]
    public float smoothSpeed = 5f;

    [Header("Offset")]
    [Tooltip("Vertical offset so the player isn't dead centre (e.g. 1 = camera sits higher).")]
    public Vector2 offset = new Vector2(0f, 1f);

    [Header("Lookahead")]
    [Tooltip("How far ahead the camera peeks in the direction the player is moving.")]
    public float lookaheadDistance = 2f;
    [Tooltip("How fast the lookahead shifts.")]
    public float lookaheadSpeed = 3f;

    [Header("Deadzone")]
    [Tooltip("Player must move this far from centre before camera starts following.")]
    public float deadzoneX = 0.5f;
    public float deadzoneY = 0.3f;

    [Header("Bounds (optional)")]
    [Tooltip("Tick to clamp the camera inside a rectangle (e.g. room bounds).")]
    public bool useBounds = false;
    public float minX, maxX, minY, maxY;

    private Vector3 _currentVelocity;
    private float _lookaheadX = 0f;
    private float _lastTargetX;

    private void LateUpdate()
    {
        if (target == null) return;

        // Lookahead based on horizontal movement
        float moveDir = target.position.x - _lastTargetX;
        if (Mathf.Abs(moveDir) > 0.01f)
            _lookaheadX = Mathf.Lerp(_lookaheadX, Mathf.Sign(moveDir) * lookaheadDistance, Time.deltaTime * lookaheadSpeed);
        _lastTargetX = target.position.x;

        // Desired position
        Vector3 desired = new Vector3(
            target.position.x + offset.x + _lookaheadX,
            target.position.y + offset.y,
            transform.position.z);

        // Apply deadzone — only move if outside it
        float dx = desired.x - transform.position.x;
        float dy = desired.y - transform.position.y;

        Vector3 goal = transform.position;
        if (Mathf.Abs(dx) > deadzoneX) goal.x = desired.x;
        if (Mathf.Abs(dy) > deadzoneY) goal.y = desired.y;

        // Smooth damp towards goal
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, goal, ref _currentVelocity, 1f / smoothSpeed);

        // Optional bounds clamping
        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
            smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);
        }

        transform.position = smoothed;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw deadzone
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadzoneX * 2f, deadzoneY * 2f, 0f));

        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 centre = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
            Vector3 size   = new Vector3(maxX - minX, maxY - minY, 0f);
            Gizmos.DrawWireCube(centre, size);
        }
    }
}
