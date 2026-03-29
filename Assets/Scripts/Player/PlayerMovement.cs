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
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetKeyDown(deathKey))
        {
            playerHealth.Die();
        }
    }

    private void FixedUpdate()
    {
        Controller.Move (HorizonatalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
