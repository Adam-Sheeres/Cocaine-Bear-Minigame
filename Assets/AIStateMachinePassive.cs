using UnityEngine;

public class AIStateMachinePassive : MonoBehaviour
{
    private Vector3 target; // target position for NPC to move to
    private int direction = 1; // direction of movement, 1 = forward, 2 = right, 3 = backward, 4 = left
    private enum State { idle, hide, run, dead, scared };
    private Vector3 upDirection = Vector3.up;

    public float distance = 5.0f; // distance traveled by NPC in each direction of the square
    public float speed = 1.0f; // speed of NPC movement

    public Transform bear;
    public SphereCollider detectionRadius;
    public SphereCollider bearTooClose;
    public AIStatus status;
    public Animator animator;

    State state = State.idle;
    bool deathHasStarted = false;

    void Start()
    {
        target = transform.position + new Vector3(0, 0, distance); // set initial target position
        status = GetComponent<AIStatus>();
    }

    void Update()
    {

        if (status.healthPoints <= 0)
        {
            state = State.dead;
        } else if (isBearTooClose())
        {
            //play dead
            state = State.scared;
        } else if (bearAround())
        {
            state = State.run;
        } else if (!bearAround())
        {
            state = State.hide;
        }

        takeActionFromState(state);
    }


    void takeActionFromState(State state)
    {
        if (state == State.idle)
        {
            GoAroundInCircles();
        } else if (state == State.hide)
        {
            hide();
        } else if (state == State.run) {
            runAway();
        } else if (state == State.dead)
        {
            die();
        } else if (state == State.scared)
        {
            hide();
        }
    }

    void die()
    {
        if (!deathHasStarted)
        {
            Debug.Log("Has Died");
            animator.SetBool("Walk", false);
            animator.SetBool("Hide", false);
            animator.SetBool("Dead", true);

            if (status.hasCocaine)
            {
                //spawn a new game component that has a cocaine script attached to it TODO
            }

            Destroy(gameObject, 4);
        }

    }

    void hide()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Hide", true);
    }

    void runAway()
    {
        animator.SetBool("Hide", false);
        animator.SetBool("Walk", true);
        Vector3 direction = transform.position - bear.position;
        direction.y = 0;

        // Rotate to look away from the bear
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
        speed = 2.0f;
        // Move away from the bear
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    void GoAroundInCircles()
    {
        // check if NPC has reached its target position
        animator.SetBool("Hide", false);
        animator.SetBool("Walk", true);
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // update direction of movement
            direction++;
            if (direction > 4) direction = 1;

            // update target position based on new direction of movement
            switch (direction)
            {
                case 1:
                    target = transform.position + new Vector3(0, 0, distance);
                    transform.LookAt(target);
                    break;
                case 2:
                    target = transform.position + new Vector3(distance, 0, 0);
                    transform.LookAt(target);
                    break;
                case 3:
                    target = transform.position + new Vector3(0, 0, -distance);
                    transform.LookAt(target);
                    break;
                case 4:
                    target = transform.position + new Vector3(-distance, 0, 0);
                    transform.LookAt(target);
                    break;
            }
        }

        // move NPC towards its target position
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
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


}
