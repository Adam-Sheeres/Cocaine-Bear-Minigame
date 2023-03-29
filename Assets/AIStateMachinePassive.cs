using UnityEngine;
using UnityEngine.Audio;

public class AIStateMachinePassive : MonoBehaviour
{
    private Vector3 target; // target position for NPC to move to
    private int direction = 1; // direction of movement, 1 = forward, 2 = right, 3 = backward, 4 = left
    private enum State { idle, Crouch, run, dead, scared, walkBack };
    private Vector3 upDirection = Vector3.up;

    public float distance = 5.0f; // distance traveled by NPC in each direction of the square
    public float speed = 1.0f; // speed of NPC movement

    public Transform bear;
    public SphereCollider detectionRadius;
    public SphereCollider bearTooClose;
    public AIStatus status;
    public Animator animator;

    public AudioSource deathSound;
    private bool cantReturn = false;
    private Vector3 startingPos;

    State state = State.idle;
    bool deathHasStarted = false;

    void Start()
    {
        target = transform.position + new Vector3(0, 0, distance); // set initial target position
        status = GetComponent<AIStatus>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            bear = player.transform;
        }
        startingPos = transform.position;
    }

    void Update()
    {

        Debug.Log(inStartingPos());

        if (status.healthPoints <= 0)
        {
            state = State.dead;
            cantReturn = true;
        } else if (isBearTooClose())
        {
            //play dead if bear is too close 
            cantReturn = true;
            state = State.scared;
        } else if (bearAround())
        {
            cantReturn = true;
            state = State.run;
        } else if (!bearAround() && cantReturn)
        {
            state = State.Crouch;
            cantReturn = true;
            Invoke("resetReturn", 5.0f);
        } else if (!cantReturn && !inStartingPos())// if you can return and you are not in the starting position
        {
            state = State.walkBack;
        } else if (inStartingPos())
        {
            state = State.idle;
        }

        takeActionFromState(state);
    }

    void resetReturn()
    {
        cantReturn = false;
    }

    void returnToStartingPos()
    {
        animator.SetBool("Hide", false);
        animator.SetBool("Crouch", false);
        animator.SetBool("Walk", true);

        Vector3 direction = startingPos - transform.position;
        direction.y = 0;

        // Rotate to look away from the bear
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
        // Move away from the bear
        transform.position += direction.normalized * speed * Time.deltaTime;
    }


    void takeActionFromState(State state)
    {
        Debug.Log(state);
        if (state == State.idle)
        {
            //do nothing
            animator.SetBool("Hide", false);
            animator.SetBool("Crouch", false);
            animator.SetBool("Walk", false);

            animator.SetBool("Idle", true);
        }
        else if (state == State.Crouch)
        {
            animator.SetBool("Idle", false);
            Crouch();
        } else if (state == State.run) {
            animator.SetBool("Idle", false);
            runAway();
        } else if (state == State.dead)
        {
            animator.SetBool("Idle", false);
            die();
        } else if (state == State.scared)
        {
            animator.SetBool("Idle", false);
            Scared();
        } else if (state == State.walkBack)
        {
            animator.SetBool("Idle", false);
            returnToStartingPos();
        }
    }

    void die()
    {
        if (!deathHasStarted)
        {
            deathHasStarted = true;
            animator.SetBool("Hide", false);
            animator.SetBool("Crouch", false);
            animator.SetBool("Walk", false);

            animator.SetBool("Dead", true);

            deathSound.Play();

            // Get a reference to the Rigidbody component
            Rigidbody rigidbody = GetComponent<Rigidbody>();

            // Disable gravity on the Rigidbody
            rigidbody.useGravity = false;


            // Get a reference to the Capsule Collider component
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            // Disable the Capsule Collider component
            capsuleCollider.enabled = false;


        }
    }

    void Scared()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Hide", true);
    }

    void Crouch()
    {
        animator.SetBool("Hide", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Crouch", true);
    }

    void runAway()
    {
        animator.SetBool("Hide", false);
        animator.SetBool("Crouch", false);
        animator.SetBool("Walk", true);
        Vector3 direction = transform.position - bear.position;
        direction.y = 0;

        // Rotate to look away from the bear
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
        // Move away from the bear
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    bool bearAround()
    {
        // Check if the bear is inside the detection radius of the object's collider
        if (Vector3.Distance(transform.position, bear.position) <= detectionRadius.radius)
        {
            // The bear is inside the detection radius
            return true;
        }
        else
        {
            // The bear is outside the detection radius
            return false;
        }
    }

    bool isBearTooClose()
    {
        if (Vector3.Distance(transform.position, bear.position) <= bearTooClose.radius)
        {
            return true;    
        } else
        {
            return false;
        }
    }

    bool inStartingPos()
    {
        float distance = Vector3.Distance(startingPos, transform.position);
        // Check if the distance is less than or equal to 1
        if (distance <= 1.0f)
        {
            return true;
        }
        return false;
    }


}
