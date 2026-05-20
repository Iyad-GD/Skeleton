using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerHealth playerHealth;

    [Header("Settings")]
    [Tooltip("Key the player must press to die.")]
    public KeyCode deathKey = KeyCode.R;

    public CharacterController2D Controller;
    public Animator animator;
    float HorizonatalMove = 0f;
    public float RunSpeed = 40f;
    bool jump = false;

    private WallJump _wallJump;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        _wallJump = GetComponent<WallJump>();
    }

    void Update()
    {
        // Don't override horizontal input during wall jump lockout
        if (_wallJump == null || !_wallJump.ControlLocked)
            HorizonatalMove = Input.GetAxisRaw("Horizontal") * RunSpeed;
        else
            HorizonatalMove = 0f;

        animator.SetFloat("Speed", Mathf.Abs(HorizonatalMove));

        if (Input.GetButtonDown("Jump"))
        {
            // Wall jump takes priority over normal jump
            if (_wallJump != null && _wallJump.IsWallSliding)
            {
                _wallJump.TriggerWallJump();
                animator.SetBool("IsJumping", true);
            }
            else
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }

        if (Input.GetKeyDown(deathKey))
            playerHealth.Die();
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    private void FixedUpdate()
    {
        Controller.Move(HorizonatalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
