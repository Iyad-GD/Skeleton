using System.Collections;
using System.Collections.Generic;
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
    public float RunSpeed =40f ;
    bool jump = false;


    // Start is called before the first frame update
    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizonatalMove = Input.GetAxisRaw("Horizontal") * RunSpeed;
        animator.SetFloat("Speed", Mathf.Abs(HorizonatalMove));
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);    
        }
        if (Input.GetKeyDown(deathKey))
        {
            playerHealth.Die();
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    private void FixedUpdate()
    {
        Controller.Move (HorizonatalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
