using UnityEngine;

public class CashPickUpPlace : MonoBehaviour
{
    [Header("Item Settings")]
    public GameObject cashOnTable; // The cash on the table
    public GameObject cashOnPlayer; // The cash to show on player
    public GameObject pickupPromptText; // UI text prompt for pickup

    [Header("Interaction Settings")]
    public float interactionDistance = 5f; // How far the player can interact from
    public LayerMask interactionLayer; // Layer for the raycast

    [Header("Movement Detection")]
    public Transform objectToTrack; // The object to track for movement
    public float movementThreshold = 0.01f; // How much movement is required to show cash

    private CigsPlacementArea cigsPlacementArea;
    private Vector3 lastObjectPosition;
    private bool objectHasMoved = false;

    private bool isHoldingCash = false;
    private Camera playerCamera;

    void Start()
    {
        // Make sure the cash on player is hidden initially
        if (cashOnPlayer != null)
        {
            cashOnPlayer.SetActive(false);
        }

        // Hide cash on table initially - it will only appear when the tracked object moves
        if (cashOnTable != null)
        {
            cashOnTable.SetActive(false);
        }

        // Hide the pickup prompt initially
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }

        cigsPlacementArea = FindObjectOfType<CigsPlacementArea>();

        // Get the main camera (usually the player's camera)
        playerCamera = Camera.main;

        // Initialize the last position of the tracked object
        if (objectToTrack != null)
        {
            lastObjectPosition = objectToTrack.position;
        }
    }

    void Update()
    {
        // Check if the tracked object has moved
        CheckObjectMovement();

        if (cigsPlacementArea == null || !cigsPlacementArea.cigsPlaced)
        {
            // Don't allow interaction yet
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(false);
            }
            return;
        }

        // Don't check for interaction if already holding cash
        if (isHoldingCash)
        {
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(false);
            }
            return;
        }

        // If cash is not on table, don't show prompt or check for interaction
        if (cashOnTable == null || !cashOnTable.activeSelf)
        {
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(false);
            }
            return;
        }

        // Raycast from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Check if the ray hits this object within the interaction distance
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            if (hit.collider.gameObject == this.gameObject || hit.collider.transform.IsChildOf(transform))
            {
                // Player is looking at the cash and in range
                if (pickupPromptText != null)
                {
                    pickupPromptText.SetActive(true);
                }

                // Check for E key press to pick up
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpCash();
                }
            }
            else
            {
                // Player is looking at something else
                if (pickupPromptText != null)
                {
                    pickupPromptText.SetActive(false);
                }
            }
        }
        else
        {
            // Nothing in range of the raycast
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(false);
            }
        }
    }

    private void CheckObjectMovement()
    {
        // Skip if no object is assigned to track
        if (objectToTrack == null)
            return;

        // Check if the object has moved beyond the threshold
        float movementDistance = Vector3.Distance(objectToTrack.position, lastObjectPosition);

        if (movementDistance > movementThreshold && !objectHasMoved)
        {
            // Object has moved, make the cash visible
            objectHasMoved = true;
            if (cashOnTable != null)
            {
                cashOnTable.SetActive(true);
            }
        }

        // Update the last position for the next frame
        lastObjectPosition = objectToTrack.position;
    }

    private void PickUpCash()
    {
        // Hide cash on table
        if (cashOnTable != null)
        {
            cashOnTable.SetActive(false);
        }

        // Show the cash on player
        if (cashOnPlayer != null)
        {
            cashOnPlayer.SetActive(true);
        }

        // Mark as picked up
        isHoldingCash = true;

        // Hide the prompt
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }
    }

    // Public method for the placement script to check
    public bool IsHoldingCash()
    {
        return isHoldingCash;
    }

    // Public method for the placement script to call when cash is placed
    public void CashPlaced()
    {
        isHoldingCash = false;
    }
}