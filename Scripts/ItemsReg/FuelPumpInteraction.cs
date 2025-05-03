using UnityEngine;

public class FuelPumpInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 5f;
    public LayerMask interactionLayer;
    public GameObject interactionPrompt;

    [Header("Audio")]
    public AudioSource pumpAudioSource;

    private bool isPlayerNearby = false;
    private bool hasBeenUsed = false;
    private bool isInteractionEnabled = false;
    private Camera mainCamera;

    // Reference to cigarette placement script
    private CigsPlacementArea cigsPlacementScript;

    void Start()
    {
        // Hide prompt initially
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Get main camera reference once
        mainCamera = Camera.main;

        // Setup audio source if needed
        if (pumpAudioSource == null)
            pumpAudioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Find cigarette placement script
        cigsPlacementScript = FindObjectOfType<CigsPlacementArea>();
    }

    void Update()
    {
        // Check if cigs have been placed - this enables the pump
        if (cigsPlacementScript != null && cigsPlacementScript.cigsPlaced)
            isInteractionEnabled = true;

        // Don't process interactions if not enabled
        if (!isInteractionEnabled || hasBeenUsed)
            return;

        // Check if player can interact
        bool canInteract = CheckRaycastInteraction() || isPlayerNearby;

        // Show/hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(canInteract);

        // Handle interaction input
        if (canInteract && Input.GetKeyDown(KeyCode.Mouse0))
            InteractWithPump();
    }

    private bool CheckRaycastInteraction()
    {
        if (mainCamera == null)
            return false;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Debug visualization
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        // Check if raycast hits this object
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
            return hit.collider.gameObject == gameObject;

        return false;
    }

    private void InteractWithPump()
    {
        hasBeenUsed = true;

        if (pumpAudioSource != null && !pumpAudioSource.isPlaying)
            pumpAudioSource.Play();

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    // Trigger zone detection for proximity-based interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenUsed && isInteractionEnabled)
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (interactionPrompt != null && !hasBeenUsed)
                interactionPrompt.SetActive(false);
        }
    }

    public bool HasBeenUsed()
    {
        return hasBeenUsed;
    }
}