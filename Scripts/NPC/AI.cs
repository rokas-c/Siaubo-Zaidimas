using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class AI : MonoBehaviour
{
    NavMeshAgent nm;
    Rigidbody rb;
    Animator animator;
    public Transform Target;
    public Transform[] WayPoints;
    public int Cur_WayPoints;
    public int speed, stop_distance;
    public float PauseTimer;
    [SerializeField]
    private float cur_timer;

    // Reference to the model to hide
    [SerializeField]
    private GameObject modelToHide;

    // Flag to check if we're at the last waypoint
    private bool isAtLastWaypoint = false;

    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;

        Target = WayPoints[Cur_WayPoints];
        cur_timer = PauseTimer;

        // If modelToHide isn't set, use this gameObject
        if (modelToHide == null)
        {
            modelToHide = gameObject;
        }
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
        }
        else if (distance <= stop_distance && WayPoints.Length > 0)
        {
            if (cur_timer > 0)
            {
                cur_timer -= 0.01f;
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsIdle", true);

                // Check if we're at the last waypoint
                if (Cur_WayPoints == WayPoints.Length - 1 && !isAtLastWaypoint)
                {
                    isAtLastWaypoint = true;
                    HideModel();
                }
            }
            if (cur_timer <= 0)
            {
                Cur_WayPoints++;
                if (Cur_WayPoints >= WayPoints.Length)
                {
                    Cur_WayPoints = 0;
                    // Reset the flag when we start a new cycle
                    isAtLastWaypoint = false;
                }
                Target = WayPoints[Cur_WayPoints];
                cur_timer = PauseTimer;
            }
        }

        nm.SetDestination(Target.position);
    }

    // Hide the model
    private void HideModel()
    {
        // Check for renderer components
        Renderer[] renderers = modelToHide.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Optionally disable the animator to stop animations
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}