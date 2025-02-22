using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lebron2dMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Vector2 dir, currVel;
    [SerializeField] private float speed, dashSpeed, jumpHeight, gravityConstant, fallSpeed;
    [SerializeField] private bool jumping, grounded;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityConstant = Physics2D.gravity.y;
        Debug.Log(gravityConstant);
    }

    // Update is called once per frame
    void Update()
    {
        dir = Input.GetAxisRaw("Horizontal") * transform.right * speed;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);
            Debug.Log("dash?");
        }
        if (!jumping && Input.GetKeyDown(KeyCode.Space))
        {
            jumping = true;
        }
    }

    private void FixedUpdate()
    {
        currVel = rb.velocity;
        currVel.x = Mathf.MoveTowards(currVel.x, dir.x, speed);
        if (jumping && grounded)
        {
            currVel.y += Mathf.Sqrt(-2f * gravityConstant * jumpHeight);
        }
        if (currVel.y < 0)
        {
            currVel += Vector2.up * gravityConstant * fallSpeed * Time.deltaTime;
        }
        rb.velocity = currVel;
        grounded = false;
        jumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckCollision(collision);
    }

    private void CheckCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            grounded |= normal.y >= 0.9f;
        }
    }
}
