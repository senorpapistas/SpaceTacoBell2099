using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;

    [Header("Private Variables")]
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;


    [Header("Variables")]
    public float speed;
    public float jumpPower;
    public float dashPower;
    public float dashTime;

    [Header("Player")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //---Recieve Inputs---//
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (Input.GetButtonDown("Fire1") && canDash)
        {
            StartCoroutine(Dash());
        }

        //need to track direction
        if (horizontal > 0) { isFacingRight = true; }
        else if (horizontal < 0) { isFacingRight = false; }

        //flip sprite
        if (horizontal >0 && !isDashing) { sprite.flipX = false; }
        else if (horizontal < 0 && !isDashing) { sprite.flipX = true; }

    }

    //---apply movement---//
    private void FixedUpdate()
    {
        if (isDashing) { return; }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        Collider2D check = Physics2D.OverlapCircle(groundCheck.position, .2f);

        if ( check != null && check.tag == "Ground")
        {
            return true;
        }
        return false;
    }

    private IEnumerator Dash()
    {
        //dash
        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        if (horizontal == 0 && vertical == 0) 
        {
            if (isFacingRight) { direction.x = 1; }
            else { direction.x = -1; }
        }

        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = direction * dashPower;
        yield return new WaitForSeconds(dashTime);

        //reset variables
        isDashing = false;
        rb.gravityScale = originalGravity;
        canDash = true;
        rb.velocity = new Vector2(0, 0);
    }
}
