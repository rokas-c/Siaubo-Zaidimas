using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CarController : MonoBehaviour
{
    // References for movement system - matching exactly with AI script
    private NavMeshAgent nm;
    public Transform Target;
    public Transform[] WayPoints;
    public int Cur_WayPoints = 0;
    public int speed = 5, stop_distance = 1;
    public float PauseTimer = 2f;
    [SerializeField]
    private float cur_timer;

    // Reference to the car model to hide
    [SerializeField]
    private GameObject modelToHide;

    // Flags - using same structure as AI script
    private bool isDriving = false;
    private bool isAtLastWaypoint = false;

    void Start()
    {
        // Get NavMeshAgent component
        nm = GetComponent<NavMeshAgent>();

        // If modelToHide isn't set, use this gameObject
        if (modelToHide == null)
        {
            modelToHide = gameObject;
        }

        // Initialize the timer
        cur_timer = PauseTimer;

        // Initialize target if we have waypoints
        if (WayPoints != null && WayPoints.Length > 0)
        {
            Target = WayPoints[Cur_WayPoints];
        }

        // Don't start driving automatically
        isDriving = false;
    }

    void Update()
    {
        if (!isDriving)
            return;

        // EXACT SAME MOVEMENT CODE AS AI SCRIPT
        //Settings Updated
        nm.acceleration = speed;
        nm.speed = speed;
        nm.stoppingDistance = stop_distance;

        float distance = Vector3.Distance(transform.position, Target.position);

        //Move to Waypoint
        if (distance > stop_distance && WayPoints.Length > 0)
        {
            //Find Waypoint
            Target = WayPoints[Cur_WayPoints];
            nm.SetDestination(Target.position);
        }
        else if (distance <= stop_distance && WayPoints.Length > 0)
        {
            if (cur_timer > 0)
            {
                cur_timer -= 0.01f;

                // Check if we're at the last waypoint
                if (Cur_WayPoints == WayPoints.Length - 1 && !isAtLastWaypoint)
                {
                    isAtLastWaypoint = true;
                    HideCarModel();
                }
            }
            if (cur_timer <= 0)
            {
                Cur_WayPoints++;
                if (Cur_WayPoints >= WayPoints.Length)
                {
                    Cur_WayPoints = 0;
                    // Reset the flags when we start a new cycle
                    isAtLastWaypoint = false;
                }
                Target = WayPoints[Cur_WayPoints];
                cur_timer = PauseTimer;

                // Important: Set the new destination
                nm.SetDestination(Target.position);
            }
        }
    }

    // Method to be called from the AI script when NPC disappears
    public void StartDriving()
    {
        isDriving = true;

        // Make sure we have the first waypoint set
        Cur_WayPoints = 0;
        cur_timer = PauseTimer;
        isAtLastWaypoint = false;

        if (WayPoints != null && WayPoints.Length > 0)
        {
            Target = WayPoints[Cur_WayPoints];
            nm.SetDestination(Target.position);
        }

        Debug.Log("Car started driving!");
    }

    // Hide the car model
    private void HideCarModel()
    {
        // Check for renderer components
        Renderer[] renderers = modelToHide.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Debug.Log("Car model hidden!");
    }
}