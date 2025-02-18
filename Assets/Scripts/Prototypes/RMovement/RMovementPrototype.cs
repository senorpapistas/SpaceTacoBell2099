using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMovementPrototype : MonoBehaviour
{
    public float speed = 1.0f;
    public float jumpForce = 9.0f;
    public Rigidbody rb;
    private Vector3 movement;
    private Vector3 facing = Vector3.right;

    [SerializeField]
    //private bool isGrounded;
    private float distToGround;
    private Vector3 boxSize;
    private Vector3 adjustedBoxSize;

    [SerializeField]
    private float dashSpeed;
    public float maxDashDist;
    private bool allowedToDash = true;


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

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && allowedToDash)
        {
            Debug.Log("trying to dash");
            StartCoroutine(Dash());
        }

    }

    private void FixedUpdate()
    {
        moveCharacter(movement);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator Dash()
    {
        Debug.Log("called dash");
        allowedToDash = false;
        Vector3 dashDirection = facing;

        RaycastHit hit;
        float dashDistance = maxDashDist;

        if (Physics.BoxCast(transform.position, boxSize / 2, dashDirection, out hit, Quaternion.identity, maxDashDist))
        {
            dashDistance = hit.distance - 0.1f;
            Debug.Log("confirmed hit, reducing dashDistance to " + dashDistance);
        }

        Vector3 dashTarget = transform.position + dashDirection * dashDistance;
        rb.MovePosition(dashTarget);

        Debug.Log("dash end location at" + dashTarget);

        yield return new WaitForSeconds(0.5f);
        Debug.Log("end dash");
        allowedToDash = true;
    }

    private void moveCharacter(Vector3 direction)
    {
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, 0);
    }

    private bool IsGrounded()
    {
        
        return Physics.BoxCast(transform.position, adjustedBoxSize / 2, -Vector3.up, Quaternion.identity, distToGround + 0.1f);
    }
}
