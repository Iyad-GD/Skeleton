using UnityEngine;


public class CarrySystem : MonoBehaviour
{
    [Header("Carry Settings")]
    [Tooltip("Where the carried object sits relative to the player (e.g. 0.5 units above).")]
    public Vector2 carryOffset = new Vector2(0f, 0.8f);

    [Tooltip("Key to pick up / drop.")]
    public KeyCode pickupKey = KeyCode.E;

    [Tooltip("Key to throw.")]
    public KeyCode throwKey = KeyCode.Mouse0;

    [Header("Throw Settings")]
    public float throwForce = 10f;

    [Tooltip("Upward angle added to the throw direction.")]
    public float throwAngleUp = 15f;

    [Header("Detection")]
    public float pickupRadius = 1.2f;
    public LayerMask carriableLayer;

    private Carriable _carried = null;
    private bool _facingRight = true;

    private void Update()
    {
        TrackFacing();

        if (Input.GetKeyDown(pickupKey))
        {
            if (_carried == null)
                TryPickUp();
            else
                Drop();
        }

        if (Input.GetKeyDown(throwKey) && _carried != null)
            Throw();

        if (_carried != null)
            MoveCarriedObject();
    }

    private void TrackFacing()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0f) _facingRight = true;
        else if (h < 0f) _facingRight = false;
    }

    private void TryPickUp()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, carriableLayer);
        float closest = float.MaxValue;
        Carriable target = null;

        foreach (var hit in hits)
        {
            Carriable c = hit.GetComponent<Carriable>();
            if (c == null || !c.canBePickedUp) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closest) { closest = dist; target = c; }
        }

        if (target != null)
            PickUp(target);
    }

    private void PickUp(Carriable carriable)
    {
        _carried = carriable;
        _carried.OnPickedUp();
    }

    private void Drop()
    {
        _carried.OnDropped(Vector2.zero);
        _carried = null;
    }

    private void Throw()
    {
        Vector2 dir = (_facingRight ? Vector2.right : Vector2.left);
        // Add upward angle
        dir = Rotate(dir, _facingRight ? throwAngleUp : -throwAngleUp);
        _carried.OnDropped(dir * throwForce);
        _carried = null;
    }

    private void MoveCarriedObject()
    {
        Vector2 offset = carryOffset;
        if (!_facingRight) offset.x = -offset.x;
        _carried.transform.position = (Vector2)transform.position + offset;
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
