using UnityEngine;

public class CigsPlacementArea : MonoBehaviour
{
    [SerializeField] private GameObject placementPromptText; // UI text prompt for placement
    [SerializeField] private GameObject cigsOnPlayer; // Reference to the cigs object on player that needs to be hidden
    [SerializeField] private float interactionDistance = 5f; // How far the player can interact from
    [SerializeField] private Collider placementCollider; // Specific collider to look at for placement

    private Camera playerCamera;
    private CigsPickUp cigsPickUpScript;
    private bool isLookingAtPlacementArea = false;

    void Start()
    {
        playerCamera = Camera.main;
        cigsPickUpScript = FindObjectOfType<CigsPickUp>();

        // If no specific collider is assigned, use this object's collider
        if (placementCollider == null)
        {
            placementCollider = GetComponent<Collider>();
        }

        // Hide the placement prompt initially
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }
    }

    void Update()
    {
        // Only check for placement if player is holding cigs
        if (cigsPickUpScript == null || !IsHoldingCigs())
        {
            if (placementPromptText != null && placementPromptText.activeInHierarchy)
            {
                placementPromptText.SetActive(false);
            }
            return;
        }

        isLookingAtPlacementArea = CheckIfLookingAtPlacementArea();

        // Update prompt visibility
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(isLookingAtPlacementArea);
        }

        // Check for E key press to place cigs
        if (isLookingAtPlacementArea && Input.GetKeyDown(KeyCode.E))
        {
            PlaceCigs();
        }
    }

    private bool IsHoldingCigs()
    {
        // Check if cigs object is active
        return cigsOnPlayer != null && cigsOnPlayer.activeInHierarchy;
    }

    private bool CheckIfLookingAtPlacementArea()
    {
        if (placementCollider == null)
            return false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // First check with a direct raycast for precision
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider == placementCollider)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                return true;
            }
        }

        // If direct raycast fails, try a slightly more forgiving approach with multiple raycasts
        float offset = 0.1f;
        Vector3[] checkPoints = new Vector3[]
        {
            new Vector3(0.5f, 0.5f, 0),           // Center
            new Vector3(0.5f + offset, 0.5f, 0),  // Right
            new Vector3(0.5f - offset, 0.5f, 0),  // Left
            new Vector3(0.5f, 0.5f + offset, 0),  // Up
            new Vector3(0.5f, 0.5f - offset, 0)   // Down
        };

        foreach (Vector3 checkPoint in checkPoints)
        {
            Ray offsetRay = playerCamera.ViewportPointToRay(checkPoint);
            if (Physics.Raycast(offsetRay, out hit, interactionDistance))
            {
                if (hit.collider == placementCollider)
                {
                    Debug.DrawLine(offsetRay.origin, hit.point, Color.yellow);
                    return true;
                }
            }
        }

        return false;
    }

    private void PlaceCigs()
    {
        if (!IsHoldingCigs())
        {
            return;
        }

        // Directly hide the cigs object on player
        if (cigsOnPlayer != null)
        {
            cigsOnPlayer.SetActive(false);
        }

        // Update the pickup script if it exists
        if (cigsPickUpScript != null)
        {
            // Call a method on CigsPickUp if it exists
            var placementMethod = cigsPickUpScript.GetType().GetMethod("OnCigsPlaced");
            if (placementMethod != null)
            {
                placementMethod.Invoke(cigsPickUpScript, null);
            }
            else
            {
                // Try to set a field directly as fallback
                var field = cigsPickUpScript.GetType().GetField("isPickedUp");
                if (field != null)
                {
                    field.SetValue(cigsPickUpScript, false);
                }
            }
        }

        // Hide the prompt
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }
    }
}