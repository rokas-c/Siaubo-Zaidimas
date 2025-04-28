using UnityEngine;

public class PunchInAction : MonoBehaviour
{
    public GameObject PunchInText;
    public AudioClip punchInSound;
    public float interactionDistance = 5f; // Maximum distance for raycast interaction
    public LayerMask interactionLayer; // Layer mask for the raycast

    // Static variable accessible by any script
    public static bool HasPunchedIn = false;

    private bool isPlayerNearby = false;
    private bool isTriggerEnabled = true; // Flag to check if trigger is enabled

    void Start()
    {
        if (PunchInText != null)
            PunchInText.SetActive(false);
    }

    void Update()
    {
        // Raycast interaction
        bool canInteract = CheckRaycastInteraction() || isPlayerNearby;

        // Show interaction text if player can interact and hasn't punched in yet
        if (canInteract && !HasPunchedIn && PunchInText != null)
        {
            PunchInText.SetActive(true);
        }
        else if (!canInteract && PunchInText != null)
        {
            PunchInText.SetActive(false);
        }

        // Check for interaction input
        if (canInteract && !HasPunchedIn && Input.GetKeyDown(KeyCode.E))
        {
            // Set static flag
            HasPunchedIn = true;

            // Play sound
            if (punchInSound != null)
                AudioSource.PlayClipAtPoint(punchInSound, transform.position);

            // Hide text
            if (PunchInText != null)
                PunchInText.SetActive(false);

            // Lock all doors when punched in
            LockAllDoors();
        }
    }

    private bool CheckRaycastInteraction()
    {
        // Cast a ray from the main camera in the forward direction
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return false;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Add debug ray to visualize in scene view
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        // First check if raycast hits anything at all (to debug)
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {

            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private void LockAllDoors()
    {
        // Find all door objects with the HingedDoorController script
        HingedDoorController[] allDoors = FindObjectsOfType<HingedDoorController>();

        // Lock each door
        foreach (HingedDoorController door in allDoors)
        {
            door.LockDoor(true); // Pass true to indicate locked by punch-in
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!HasPunchedIn && PunchInText != null)
                PunchInText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (PunchInText != null && !CheckRaycastInteraction())
                PunchInText.SetActive(false);
        }
    }

    // This can be called from the inspector or other scripts to check if isTrigger is enabled
    public bool IsTriggerEnabled()
    {
        return isTriggerEnabled;
    }

    // Called in OnDrawGizmos or OnDrawGizmosSelected to visualize the interaction range
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere to visualize the interaction distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}