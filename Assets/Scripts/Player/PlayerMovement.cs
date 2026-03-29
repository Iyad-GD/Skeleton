using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

        
    public CharacterController2D Controller;
    float HorizonatalMove = 0f;
    public float RunSpeed =40f ;
    bool jump = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HorizonatalMove = Input.GetAxisRaw("Horizontal") * RunSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        Controller.Move (HorizonatalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
