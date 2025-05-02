using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class AI : MonoBehaviour
{
    NavMeshAgent nm;
    Rigidbody rb;
    Animator animator; // Changed from Animation to Animator
    public Transform Target;
    public Transform[] WayPoints;
    public int Cur_WayPoints;
    public int speed, stop_distance;
    public float PauseTimer;
    [SerializeField]
    private float cur_timer;

    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Initialize the Animator component

        rb.freezeRotation = true;

        Target = WayPoints[Cur_WayPoints];
        cur_timer = PauseTimer;
    }

    void Update()
    {
        //Settings Updated
        nm.acceleration = speed;
        nm.stoppingDistance = stop_distance;

        float distance = Vector3.Distance(transform.position, Target.position);


        //Move to Waypoint
        if (distance > stop_distance && WayPoints.Length > 0)
        {
            animator.SetBool("IsMoving", true);
            animator.SetBool("IsIdle", false);
            //Find Waypoint
            Target = WayPoints[Cur_WayPoints];

            Debug.Log("Hello");
        }
        else if (distance <= stop_distance && WayPoints.Length > 0)
        {
            if (cur_timer > 0)
            {
                cur_timer -= 0.01f;
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsIdle", true);
            }
            if (cur_timer <= 0)
            {
                Cur_WayPoints++;
                if (Cur_WayPoints >= WayPoints.Length)
                {
                    Cur_WayPoints = 0;
                }
                Target = WayPoints[Cur_WayPoints];
                cur_timer = PauseTimer;
            }
        }

        nm.SetDestination(Target.position);
    }
}