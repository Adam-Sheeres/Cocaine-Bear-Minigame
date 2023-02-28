using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator.SetBool("Idle", true);
    }
}
