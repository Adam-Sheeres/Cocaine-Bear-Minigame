using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    public Animator animator;
    public ArrayList hidingSpots;
    public int speed = 10;
    public Transform bearLocation;
    private Rigidbody rb;
    public CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        //start by running away from bear until a safe distance
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, bearLocation.position) <= 10.0f)
        {
            Vector3 direction = transform.position - bearLocation.position;
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

    }
}