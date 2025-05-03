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

    // Flag to track if we're at the last waypoint
    private bool hasReachedLastWaypoint = false;

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
            Debug.Log("No model specified to hide, using this gameObject");
        }

        Debug.Log("AI script initialized. Last waypoint index: " + (WayPoints.Length - 1));
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
            // Check if we're at the last waypoint
            bool isLastWaypoint = (Cur_WayPoints == WayPoints.Length - 1);

            if (isLastWaypoint && !hasReachedLastWaypoint)
            {
                Debug.Log("Reached last waypoint! Hiding model now.");
                HideModel();
                hasReachedLastWaypoint = true;
            }

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
                    // Don't reset hasReachedLastWaypoint flag - we want the model to stay hidden
                }
                Target = WayPoints[Cur_WayPoints];
                cur_timer = PauseTimer;

                Debug.Log("Moving to waypoint index: " + Cur_WayPoints);
            }
        }

        nm.SetDestination(Target.position);
    }

    // Hide the model
    private void HideModel()
    {
        // Simply disable the entire GameObject
        modelToHide.SetActive(false);
    }
}