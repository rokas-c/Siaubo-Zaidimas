using UnityEngine;

public class CashPlacementArea : MonoBehaviour
{
    [SerializeField] private GameObject placementPromptText; // UI text prompt for placement
    [SerializeField] private GameObject cashOnPlayer; // Reference to the cash object on player that needs to be hidden
    [SerializeField] private float interactionDistance = 5f; // How far the player can interact from
    [SerializeField] private float sphereCastRadius = 0.5f; // Radius of the sphere cast for more forgiving detection
    
    private Camera playerCamera;
    private CashPickUpPlace cashPickUpScript;
    
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
        // Only check for placement if player is holding cash
        if (cashPickUpScript == null || !cashPickUpScript.IsHoldingCash())
        {
            if (placementPromptText != null)
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
        if (isLookingAtPlacementArea && Input.GetKeyDown(KeyCode.E))
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
        
        // Tell the pickup script that cash has been placed
        cashPickUpScript.CashPlaced();
        
        // Hide the prompt
        if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }
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