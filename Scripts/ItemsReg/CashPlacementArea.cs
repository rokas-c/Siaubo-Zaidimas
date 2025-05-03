using UnityEngine;

public class CashPlacementArea : MonoBehaviour
{
    public GameObject placementPromptText; // UI text prompt for placement
    public GameObject cashOnPlayer; // Reference to the cash object on player that needs to be hidden
    public float interactionDistance = 5f; // How far the player can interact from
    public float sphereCastRadius = 0.5f; // Radius of the sphere cast for more forgiving detection
    public AudioSource PlacementSound;

    private Camera playerCamera;
    private CashPickUpPlace cashPickUpScript;
    private bool cashPlacedPermanently = false; // Flag to track if cash has been placed permanently

    void Start()
    {
        playerCamera = Camera.main;
        cashPickUpScript = FindObjectOfType<CashPickUpPlace>();

        // Hide the placement prompt initially
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }
    }

    void Update()
    {
        // If cash was already placed permanently, don't process anything
        if (cashPlacedPermanently)
        {
            return;
        }

        // Only check for placement if player is holding cash
        if (cashPickUpScript == null || !cashPickUpScript.IsHoldingCash())
        {
            if (placementPromptText != null && placementPromptText.activeSelf)
            {
                placementPromptText.SetActive(false);
            }
            return;
        }

        bool isLookingAtPlacementArea = CheckIfLookingAtPlacementArea();

        // Update prompt visibility
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(isLookingAtPlacementArea);
        }

        // Check for E key press to place cash
        if (isLookingAtPlacementArea && Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceCash();
        }
    }

    private bool CheckIfLookingAtPlacementArea()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Use SphereCast instead of Raycast for more forgiving detection
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, interactionDistance))
        {
            // Check if we hit this object or any of its children
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    private void PlaceCash()
    {
        if (cashPickUpScript == null || !cashPickUpScript.IsHoldingCash())
        {
            return;
        }

        // Directly hide the cash object on player
        if (cashOnPlayer != null)
        {
            cashOnPlayer.SetActive(false);
        }

        // Play placement sound if we have one
        if (PlacementSound != null)
        {
            PlacementSound.Play();
        }

        // Hide the prompt
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }

        // Mark as permanently placed
        cashPlacedPermanently = true;

        // Optional: Disable collider to prevent further interaction
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    // Public method for other scripts that might need to know if cash was placed permanently
    public bool IsCashPlacedPermanently()
    {
        return cashPlacedPermanently;
    }

    // Optional: Add visual debugging to see what's happening
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ray.origin + ray.direction * interactionDistance, sphereCastRadius);
        }
    }
}