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
    public float attackCooldown;

    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Other")]
    public Transform orientation;
    public Animator animator;
    public SphereCollider attackRange;
    public PlayerStatus status;

    float hInput;
    float vInput;
    bool readyToJump;
    bool canAttack;
    bool endGameStarted = false;
    bool animLock = false; 

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        animator.SetBool("Idle", true);
        canAttack = true;
    }

    private void FixedUpdate()
    {
        GameObject closestEnemy = getClosestEnemy();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();

        if (status.healthPoints <= 0.0f) //health points 0
        {
            if (!endGameStarted)
            {
                endGameStarted = true;
                //die -> Game over 
                animator.SetBool("Run Forward", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Attack", false);
                animator.SetBool("Death", true);

                endGame();
            }

            //start game over sequence 
        } else if (status.cocainePoints <= 0.0f) //cocaine points 0
        {
            if (!endGameStarted)
            {
                endGameStarted = true;
                //sleep -> Game over 
                animator.SetBool("Run Forward", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Attack", false);
                animator.SetBool("Sleep", true);

                endGame();
            }
        }
        
        else
        {
            if (grounded)
            {
                rb.drag = groundDrag;
                animator.SetBool("Jumping", false);
            }
            else
            {
                rb.drag = 0;
            }

            //attack
            if (Input.GetButton("Fire1"))
            {
                animator.SetBool("Run Forward", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Attack", true);

                //if in range 
                if (IsEnemyInAttackRange(closestEnemy.transform, attackRange) && canAttack)
                {
                    canAttack = false;
                    Invoke(nameof(resetAttack), attackCooldown);
                    AIStatus aiStatus = closestEnemy.GetComponent<AIStatus>();
                    aiStatus.sendMessage("take damage", 1);
                }
            } else if (Input.GetKeyDown(KeyCode.E))
            {
                //if there is a dead body near you 
                if (IsEnemyInAttackRange(closestEnemy.transform, attackRange))
                {
                    AIStatus aiStatus = closestEnemy.transform.GetComponent<AIStatus>();
                    if (aiStatus.healthPoints <= 0.0f) //this is a dead body
                    {
                        animator.SetBool("Attack", false);
                        animator.SetBool("Eat", true);
                        animator.SetBool("Idle", false);
                        animLock = true;
                        aiStatus.sendMessage("being eaten", 0);
                        Invoke("ResetEat", 4.0f);
                    }
                }
            }
            else if (!animLock)
            {
                animator.SetBool("Attack", false);
                MovePlayer();
            }

        }
    }

    void ResetEat()
    {
        animator.SetBool("Eat", false);
        animLock = false;
        status.sendMessage("add health");
        status.sendMessage("add point");
    }

    void endGame()
    {
        FindObjectOfType<GameManager>().EndGame();
    }

    void resetAttack()
    {
        canAttack = true;
    }


    bool IsEnemyInAttackRange(Transform closestEnemy, SphereCollider attackRange)
    {
        // Calculate the distance between the attackRange's center and the closestEnemy's position
        float distanceToEnemy = Vector3.Distance(attackRange.transform.position, closestEnemy.position);

        // Check if the distance is less than the attackRange's radius
        if (distanceToEnemy < attackRange.radius)
        {
            // The enemy is within the attack range
            return true;
        }
        else
        {
            // The enemy is outside the attack range
            return false;
        }
    }


    private GameObject getClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }



    private void MyInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(resetJump), jumpCooldown); //jump cooldown
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * vInput + orientation.right * hInput;
        moveDirection.y = 0.0f; // Prevent movement in the y-axis

        // Check if the player is holding down the shift key and set the move speed accordingly
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveSpeed *= 1.5f; // Double the move speed if shift is held down
            status.setCocaineDrainRate(5.0f);
        } else
        {
            status.setCocaineDrainRate(2.5f);
        }

        // Move the player using the transform class instead of adding forces to the rigid body
        transform.position += moveDirection.normalized * currentMoveSpeed * Time.deltaTime;

        // If the player is not on the ground, apply air multiplier to the movement
        if (!grounded)
        {
            transform.position += moveDirection.normalized * currentMoveSpeed * airMultiplier * Time.deltaTime;
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

        animator.SetBool("Jumping", true);
        Debug.Log("Jumping");
    }

    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger(triggerName);
    }

    private void resetJump()
    {
        readyToJump = true;
        animator.SetBool("Jumping", false);
    }
}
