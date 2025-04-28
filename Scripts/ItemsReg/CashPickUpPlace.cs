using UnityEngine;

public class CashPickUpPlace : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject cashOnTable; // The cash on the table
    [SerializeField] private GameObject cashOnPlayer; // The cash to show on player
    [SerializeField] private GameObject pickupPromptText; // UI text prompt for pickup
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 5f; // How far the player can interact from
    [SerializeField] private LayerMask interactionLayer; // Layer for the raycast
    
    private bool isHoldingCash = false;
    private Camera playerCamera;
    
    void Start()
    {
        // Make sure the cash on player is hidden initially
        if (cashOnPlayer != null)
        {
            cashOnPlayer.SetActive(false);
        }
        
        // Show cash on table initially
        if (cashOnTable != null)
        {
            cashOnTable.SetActive(true);
        }

        // Hide the pickup prompt initially
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }
        
        // Get the main camera (usually the player's camera)
        playerCamera = Camera.main;
    }

    void Update()
    {

        
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