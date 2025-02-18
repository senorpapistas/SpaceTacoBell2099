using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMovementPrototype : MonoBehaviour
{
    // values in script are ones i thought were comfy
    [Header("Physics Values")]
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 2.7f;
    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 facing = Vector3.right;

    private float distToGround;
    private Vector3 boxSize;
    private Vector3 adjustedBoxSize;

    [Header("Dash Settings")]
    [SerializeField]
    private float dashSpeed = 30f;
    public float maxDashDist = 2.5f;
    [SerializeField]
    private float DASH_COOLDOWN = 0.4f;

    private bool allowedToDash = true;
    private bool isDashing = false;

    private KeyCode JUMP_KEY = KeyCode.Space;
    private KeyCode DASH_KEY = KeyCode.LeftShift;



    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        Collider collider = rb.GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;
        boxSize = new Vector3(collider.bounds.size.x, 0.1f, collider.bounds.size.z);
        adjustedBoxSize = new Vector3(boxSize.x * 0.9f, boxSize.y, boxSize.z * 0.9f);
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        movement = new Vector3(moveHorizontal, moveVertical, 0).normalized;

        if (movement.x != 0) // record its last facing direction
        {
            facing = movement.x > 0 ? Vector3.right : Vector3.left;
        }

        if (Input.GetKeyDown(JUMP_KEY) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(DASH_KEY) && allowedToDash) 
        {
            //Debug.Log("trying to dash");
            StartCoroutine(Dash());
        }

    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            MoveCharacter(movement);
            AdjustGravity();
        }
    }

    // dash velocity implementation
    private IEnumerator Dash()
    {
        isDashing = true;
        allowedToDash = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        Vector3 dashDirection = facing;

        RaycastHit hit;
        float dashDistance = maxDashDist;

        if (rb.SweepTest(dashDirection, out hit, maxDashDist))
        {
            dashDistance = hit.distance - 0.05f;
        }

        Vector3 dashVelocity = dashDirection * dashSpeed;
        float dashTime = dashDistance / dashSpeed;

        rb.velocity = dashVelocity;
        yield return new WaitForSeconds(dashTime);
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
        isDashing = false;
        yield return new WaitForSeconds(DASH_COOLDOWN);
        allowedToDash = true;
    }
    private void Jump()
    {
        // when falling from great heights then using jump, chance for adjusted gravity to reduce max jump height
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void MoveCharacter(Vector3 direction)
    {
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, 0);
    }

    private void AdjustGravity()
    {
        if (rb.velocity.y < 0) // increase fall speed
        { // needs to be multiplied by fixedDeltaTime
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(JUMP_KEY)) // if jump key is released before apex
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private bool IsGrounded()
    {
        return Physics.BoxCast(transform.position, adjustedBoxSize / 2, -Vector3.up, Quaternion.identity, distToGround + 0.1f);
    }
}
