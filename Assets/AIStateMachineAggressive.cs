using UnityEngine;

public class AIStateMachineAggressive : MonoBehaviour
{
    private Vector3 target; // target position for NPC to move to
    private int direction = 1; // direction of movement, 1 = forward, 2 = right, 3 = backward, 4 = left
    private enum State { idle, attack, reposition, runAway, dead};
    private Vector3 upDirection = Vector3.up;

    public float distance = 5.0f; // distance traveled by NPC in each direction of the square
    public float speed = 1.0f; // speed of NPC movement

    public float attackCooldown = 2.0f;
    bool canAttack = true;
    bool deathHasStarted = false;

    public Transform bear;
    public SphereCollider detectionRadius;
    public SphereCollider attackRadius;
    public AIStatus status;
    public Animator animator;

    string curAnimation;

    void Start()
    {
        target = transform.position + new Vector3(0, 0, distance); // set initial target position
        takeActionFromState(State.idle);
        status = GetComponent<AIStatus>();
        curAnimation = "Idle";
    }

    void Update()
    {
        State state;


        //can see and not within the minimum attack radius, OK to attack 
        if (status.healthPoints <= 0)
        {
            state = State.dead;
        } else if (inDetectionRadius() && !inAttackRadius())
        {
            state = State.attack;
            //if the bear is inside the attack radius, we want to reposition
        } else if (inDetectionRadius() && inAttackRadius())
        {
            state = State.runAway;
        } else
        {
            state = State.idle;
        }

        takeActionFromState(state);
    }


    void takeActionFromState(State state)
    {
        if (state == State.idle)
        {
            GoAroundInCircles();
        } else if (state == State.attack) {
            attackbear();
        } else if (state == State.runAway)
        {
            runAway();
        } else if (state == State.dead)
        {
            die();
        }
    }

    void changeAnimation(string animation)
    {
        if (curAnimation == animation)
        {
            return;
        } else
        {
            curAnimation = animation;
            switch (animation)
            {
                case "Hide":
                    animator.SetBool("Walk", false);
                    animator.SetBool("Attack", false);
                    animator.SetBool("Dead", false);
                    animator.SetBool("Hide", true);
                    break;
                case "Walk":
                    animator.SetBool("Hide", false);
                    animator.SetBool("Attack", false);
                    animator.SetBool("Dead", false);
                    animator.SetBool("Walk", true);
                    break;
                case "Attack":
                    animator.SetBool("Hide", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Dead", false);
                    animator.SetBool("Attack", true);
                    break;
                case "Dead":
                    animator.SetBool("Hide", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Attack", false);
                    animator.SetBool("Dead", true);
                    break;
            }
        }
    }

    void die()
    {
        if (!deathHasStarted)
        {
            deathHasStarted = true;
            Debug.Log("Has Died");
            changeAnimation("Dead");

            Destroy(gameObject, 4);
        }
    }

    void attackbear()
    {
        if (canAttack)
        {
            canAttack = false;
            changeAnimation("Attack");
            Debug.Log("Attacking Bear");

            transform.LookAt(bear);
            Invoke(nameof(resetAttackCooldown), attackCooldown);
        }
    }

    void resetAttackCooldown()
    {
        canAttack = true;
    }


    void runAway()
    {
        changeAnimation("Walk");
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
        changeAnimation("Walk");
        // check if NPC has reached its target position
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

    bool inDetectionRadius()
    {
        return Vector3.Distance(transform.position, bear.position) <= detectionRadius.radius;
    }

    bool inAttackRadius()
    {
        return Vector3.Distance(transform.position, bear.position) <= attackRadius.radius;
    }


}
