using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded; 

    public Transform orientation;
    public Animator animator;

    float hInput;
    float vInput;
    bool readyToJump;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        animator.SetBool("Idle", true);
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();

        if (grounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }
        MovePlayer();
    }


    private void MyInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * vInput + orientation.right * hInput;
        moveDirection.y = 0.0f; // Prevent movement in the y-axis

        // Move the player using the transform class instead of adding forces to the rigid body
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

        // If the player is not on the ground, apply air multiplier to the movement
        if (!grounded)
        {
            transform.position += moveDirection.normalized * moveSpeed * airMultiplier * Time.deltaTime;
        }

        // Check if the player is moving horizontally
        bool isMoving = (hInput != 0.0f || vInput != 0.0f);

        // Set the 'run forward' animation to true if the player is moving, otherwise set it to false and set 'idle' animation to true
        animator.SetBool("Run Forward", isMoving);
        animator.SetBool("Idle", !isMoving);

    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        animator.SetTrigger("Jump");
    }

    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger(triggerName);
    }

    private void resetJump()
    {
        readyToJump = true;
    }
}
