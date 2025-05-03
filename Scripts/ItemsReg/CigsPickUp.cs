using UnityEngine;

public class CigsPickUp : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject cigsInHandObject;
    [SerializeField] private GameObject pickupPromptText;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private Camera playerCamera;

    private bool isPickedUp = false;
    private bool isInteractionEnabled = false;

    void Start()
    {
        // Initialize objects
        if (cigsInHandObject != null)
            cigsInHandObject.SetActive(false);

        if (pickupPromptText != null)
            pickupPromptText.SetActive(false);

        // Get main camera if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        // Skip processing if interaction is disabled or already picked up
        if (!isInteractionEnabled || isPickedUp)
            return;

        // Check if player is looking at this object
        bool isLookingAtCigs = IsPlayerLookingAtThis();

        // Update pickup prompt visibility
        if (pickupPromptText != null)
            pickupPromptText.SetActive(isLookingAtCigs);

        // Process pickup when E is pressed
        if (isLookingAtCigs && Input.GetKeyDown(KeyCode.Mouse0))
            PickUp();
    }

    private bool IsPlayerLookingAtThis()
    {
        if (playerCamera == null)
            return false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            return hit.collider.gameObject == gameObject ||
                   hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }

    private void PickUp()
    {
        gameObject.SetActive(false);

        if (cigsInHandObject != null)
            cigsInHandObject.SetActive(true);

        isPickedUp = true;

        if (pickupPromptText != null)
            pickupPromptText.SetActive(false);
    }

    // Public method to enable interactions from other scripts
    public void EnableInteraction()
    {
        isInteractionEnabled = true;
    }
}