using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lebronMovementPrototype : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Vector2 dir, currVel;
    [SerializeField] private float speed, dashSpeed, maxAccel, jumpHeight, fallSpeed, gravityConstant, extraJumpHeight;
    [SerializeField] private bool jumping, grounded, validJump;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityConstant = Physics.gravity.y;
        Debug.Log(gravityConstant);
    }

    // Update is called once per frame
    private void Update()
    {
        dir = Input.GetAxisRaw("Horizontal") * transform.right * speed;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(dir * dashSpeed, ForceMode.Impulse);
            Debug.Log("dash?");
        }
        if (!jumping && Input.GetKeyDown(KeyCode.Space))
        {
            jumping = true;
        }//*/
        //jumping = Input.GetKey(KeyCode.Space);
    }
    void FixedUpdate()
    {
        //rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + dir * speed);
        currVel = rb.velocity;
        
        //float maxSpeed = maxAccel * Time.deltaTime;
        currVel.x = Mathf.MoveTowards(currVel.x, dir.x, speed);
        if (jumping && grounded)
        {
            currVel.y += Mathf.Sqrt(-2f * gravityConstant * jumpHeight);
        }
        /*if (jumping && currVel.y > 0)
        {
            currVel.y += jumpHeight * extraJumpHeight;
        }*/
        if (currVel.y < 0)
        {
            currVel += Vector2.up * gravityConstant * fallSpeed * Time.deltaTime;
        }
        rb.velocity = currVel;
        grounded = false;
        jumping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            grounded |= normal.y >= 0.9f;
            //jumping = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            grounded |= normal.y >= 0.9f;
            //jumping = false;
        }
    }
}
