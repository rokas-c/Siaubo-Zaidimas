using UnityEngine;

public class DetectorForItems : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform targetToWatch;
    public float movementThreshold = 0.01f;
    public CigsPickUp cigsPickUpScript;
    private Vector3 lastPosition;
    private bool hasActivated = false;

    void Start()
    {
        if (targetToWatch == null)
        {
            enabled = false;
            return;
        }

        if (cigsPickUpScript == null)
        {
            cigsPickUpScript = FindObjectOfType<CigsPickUp>();
            if (cigsPickUpScript == null)
            {
                enabled = false;
                return;
            }
        }

        // Store initial position
        lastPosition = targetToWatch.position;
    }

    void Update()
    {
        // Skip if already activated
        if (hasActivated)
            return;

        // Check if target has moved beyond threshold
        float distanceMoved = Vector3.Distance(targetToWatch.position, lastPosition);

        if (distanceMoved > movementThreshold)
        {
            // Enable interaction in CigsPickUp script
            cigsPickUpScript.EnableInteraction();
            hasActivated = true;

            // Optional: Disable this component to save resources
            enabled = false;
        }

        // Update last position
        lastPosition = targetToWatch.position;
    }
}