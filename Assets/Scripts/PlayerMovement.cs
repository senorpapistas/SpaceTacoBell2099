using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // values in script are ones i thought were comfy
    [Header("Physics Values")]
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public float apexThreshold = 1f; // velocity to start applying fall multiplier at
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 2.7f;
    public float crouchSpeedMultiplier = 0.5f;
    private Rigidbody2D rb;
    private Vector3 movement;
    private Vector3 facing = Vector2.right;

    // raycast checks
    private float distToGround;
    private float playerHeight;
    private Vector3 boxSize;
    private Vector3 adjustedBoxSize;

    [Header("Dash Settings")]
    [SerializeField]
    private float dashSpeed = 30f;
    public float maxDashDist = 2.5f;
    [SerializeField]
    private float DASH_COOLDOWN = 0.4f;

    [Header("Crouch Settings")]
    [SerializeField]
    private float crouchSpeed = 0.1f;
    [SerializeField]
    private float CROUCH_COOLDOWN = 0.30f;

    private bool allowedToDash = true;
    private bool isDashing = false;
    private bool isCrouched = false; // player crouch status
    private bool isCrouching = false; // is animation playing
    private bool canCrouch = true; // cooldown

    private KeyCode JUMP_KEY = KeyCode.Space;
    private KeyCode DASH_KEY = KeyCode.LeftShift;
    private KeyCode CROUCH_KEY = KeyCode.C;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        Collider2D collider = rb.GetComponent<Collider2D>();
        distToGround = collider.bounds.extents.y;
        playerHeight = collider.bounds.size.y;
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
            facing = movement.x > 0 ? Vector2.right : Vector2.left;
        }

        if (Input.GetKeyDown(JUMP_KEY) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(CROUCH_KEY) && canCrouch)
        {
            Debug.Log("called crouch");
            StartCoroutine(Crouch());
            //Crouch();
        }

        if (isCrouched && !Input.GetKey(CROUCH_KEY))
        {
            StartCoroutine(AttemptUncrouch());
        }

        // only allowed to dash if uncrouched, do crouch check first
        if (Input.GetKeyDown(DASH_KEY) && allowedToDash && !isCrouched)
        {
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
        //rb.useGravity = false;
        rb.velocity = Vector2.zero;
        Vector3 dashDirection = facing;

        RaycastHit hit;
        float dashDistance = maxDashDist;

        /*
        // on expected collision, set distance to before it
        if (rb.SweepTest(dashDirection, out hit, maxDashDist))
        {
            dashDistance = hit.distance - 0.05f;
        }
        */

        Vector3 dashVelocity = dashDirection * dashSpeed;
        float dashTime = dashDistance / dashSpeed;

        rb.velocity = dashVelocity;
        yield return new WaitForSeconds(dashTime);
        rb.velocity = Vector2.zero;
        //rb.useGravity = true;
        isDashing = false;
        yield return new WaitForSeconds(DASH_COOLDOWN);
        allowedToDash = true;
    }

    private void Jump()
    {
        // when falling from great heights then using jump, chance for adjusted gravity to reduce max jump height
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    //private void Crouch()
    //{
    //    if (!isCrouched)
    //    {
    //        isCrouched = true;
    //        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
    //        transform.position -= new Vector3(0, playerHeight / 4, 0); // correct players position
    //        speed *= crouchSpeedMultiplier;
    //    }
    //}

    private IEnumerator Crouch()
    {
        if (!isCrouched && !isCrouching)
        {
            isCrouched = true;
            isCrouching = true;
            canCrouch = false;
            float elapsedTime = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = transform.position - new Vector3(0, playerHeight / 4, 0);

            while (elapsedTime < crouchSpeed)
            { // crouch animation
                transform.localScale = Vector2.Lerp(startScale, targetScale, elapsedTime / crouchSpeed);
                transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / crouchSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
            transform.position = targetPosition;
            speed *= crouchSpeedMultiplier;
            isCrouching = false;
            yield return new WaitForSeconds(CROUCH_COOLDOWN);
            canCrouch = true;
        }
    }

    private IEnumerator Uncrouch()
    {
        isCrouching = true;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + new Vector3(0, playerHeight / 4, 0);

        while (elapsedTime < crouchSpeed)
        {
            transform.localScale = Vector2.Lerp(startScale, targetScale, elapsedTime / crouchSpeed);
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / crouchSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPosition;
        speed /= crouchSpeedMultiplier;
        isCrouched = false;
        isCrouching = false;
    }

    private IEnumerator AttemptUncrouch()
    {
        while (isCrouched)
        {
            if (!IsCeiling() && !isCrouching)
            {
                yield return StartCoroutine(Uncrouch());
                yield break;
            }
            yield return null;
        }
    }

    private void MoveCharacter(Vector3 direction)
    {
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, 0);
    }

    private void AdjustGravity()
    {
        if (rb.velocity.y < 0) // increase fall speed
        { // needs to be multiplied by fixedDeltaTime
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && rb.velocity.y < apexThreshold) // apply fall multiplier before apex
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(JUMP_KEY))
        { // if jump key is released before apex, decrease gravity by lowJumpMultiplier
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        // extra gravity for less floaty
        //rb.AddForce(new Vector3(0, -0.1f, 0), ForceMode.Impulse);
    }

    private bool IsGrounded()
    { // shoot a short box cast downwards with a slightly smaller box hitbox
        return Physics2D.BoxCast(transform.position, adjustedBoxSize / 2, 0, -Vector2.up, distToGround + 0.1f);
    }

    private bool IsCeiling()
    { // check if player will collide with ceiling from uncrouch
        float crouchedHeight = playerHeight / 2;
        int layerMask = ~LayerMask.GetMask("Player");
        return Physics2D.BoxCast(
            transform.position + Vector3.up * (crouchedHeight / 2), 
            adjustedBoxSize / 2, 0, 
            Vector2.up, 
            (playerHeight - crouchedHeight) + 0.1f, 
            layerMask
        );
    }
}
