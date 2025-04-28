using UnityEngine;

public class CigsPickUp : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject cigsInHandObject; // The cigarettes to show in player's hand
    [SerializeField] private GameObject pickupPromptText; // UI text prompt for pickup

    [Header("Interaction Settings")]
    [SerializeField] private float pickupRange = 5f; // How far the player can pick up from
    [SerializeField] private Camera playerCamera; // The player's camera for raycasting

    [Header("Activation Settings")]
    [SerializeField] private GameObject triggerObject; // The object to check for visibility
    [SerializeField] private float checkInterval = 0.5f; // How often to check if object is visible

    private bool isPickedUp = false;
    private float checkTimer = 0f;
    private bool isInteractionEnabled = false;

    void Start()
    {
        // Make sure the cigs in hand are hidden initially
        if (cigsInHandObject != null)
        {
            cigsInHandObject.SetActive(false);
        }

        // Hide the pickup prompt initially
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }

        // Get the main camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // Check if the trigger object is visible at the set interval
        if (!isInteractionEnabled && triggerObject != null)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0f;
                if (triggerObject.activeInHierarchy)
                {
                    isInteractionEnabled = true;
                }
            }
        }

        // Don't process interactions if not enabled
        if (!isInteractionEnabled)
        {
            return;
        }

        // Don't do anything if already picked up
        if (isPickedUp)
        {
            return;
        }

        // Check if player is looking at the cigarettes
        bool isLookingAtCigs = CheckIfLookingAtCigs();

        // Show pickup prompt only when looking at cigs within pickup range
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(isLookingAtCigs);
        }

        // Check for E key press to pick up
        if (isLookingAtCigs && Input.GetKeyDown(KeyCode.E))
        {
            PickUpCigs();
        }
    }

    private bool CheckIfLookingAtCigs()
    {
        if (playerCamera == null)
            return false;

        // Raycast from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Check if ray hits this object within the pickup range
        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // Check if we hit this object or any of its children
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    private void PickUpCigs()
    {
        // Hide this object (the cigarette pack in the world)
        gameObject.SetActive(false);

        // Show the cigarettes in hand
        if (cigsInHandObject != null)
        {
            cigsInHandObject.SetActive(true);
        }

        // Mark as picked up
        isPickedUp = true;

        // Hide the prompt
        if (pickupPromptText != null)
        {
            pickupPromptText.SetActive(false);
        }
    }
}