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

    private CigsPlacementArea cigsPlacementArea;
    private bool isHoldingCash = false;
    private Camera playerCamera;
    private bool cashSpawned = false;
    private bool cashPickedUpPermanently = false;

    void Start()
    {
        // Make sure the cash on player is hidden initially
        if (cashOnPlayer != null)
        {
            cashOnPlayer.SetActive(false);
        }

        // Hide cash on table initially - it will appear when cigs are placed
        if (cashOnTable != null)
        {
            cashOnTable.SetActive(false);
        }

        // Hide the pickup prompt initially
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }

        // Find the cigarette placement area script
        cigsPlacementArea = FindObjectOfType<CigsPlacementArea>();

        // Get the main camera (usually the player's camera)
        playerCamera = Camera.main;
    }

    void Update()
    {
        // If cash was already picked up permanently, don't process anything
        if (cashPickedUpPermanently)
        {
            return;
        }

        // Check if cigarettes have been placed and cash hasn't spawned yet
        if (cigsPlacementArea != null && cigsPlacementArea.cigsPlaced && !cashSpawned && !isHoldingCash)
        {
            SpawnCash();
        }

        // Don't check for interaction if already holding cash or cash isn't spawned
        if (isHoldingCash || !cashSpawned)
        {
            if (pickupPromptText != null && pickupPromptText.activeSelf)
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
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    PickUpCash();
                    // Mark as permanently picked up
                    cashPickedUpPermanently = true;
                }
            }
            else
            {
                // Player is looking at something else
                if (pickupPromptText != null && pickupPromptText.activeSelf)
                {
                    pickupPromptText.SetActive(false);
                }
            }
        }
        else
        {
            // Nothing in range of the raycast
            if (pickupPromptText != null && pickupPromptText.activeSelf)
            {
                pickupPromptText.SetActive(false);
            }
        }
    }

    private void SpawnCash()
    {
        // Show the cash on the table when cigarettes are placed
        if (cashOnTable != null)
        {
            cashOnTable.SetActive(true);
        }
        cashSpawned = true;
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

        // Optional: Disable collider to prevent further interaction
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    // Public method for the placement script to check
    public bool IsHoldingCash()
    {
        return isHoldingCash;
    }

    // Public method for other scripts that might need to know if cash was picked up permanently
    public bool IsCashPickedUpPermanently()
    {
        return cashPickedUpPermanently;
    }
}