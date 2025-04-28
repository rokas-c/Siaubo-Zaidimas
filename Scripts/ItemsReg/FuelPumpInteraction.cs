using UnityEngine;

public class FuelPumpInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 5f;
    public LayerMask interactionLayer;
    public GameObject interactionPrompt;

    [Header("Audio")]
    public AudioSource pumpAudioSource;

    [Header("Activation Settings")]
    public GameObject triggerObject; // The object to check for visibility
    public float checkInterval = 0.5f; // How often to check if object is visible

    private bool isPlayerNearby = false;
    private bool hasBeenUsed = false; // Track if the pump has been used already
    private float checkTimer = 0f;
    private bool isInteractionEnabled = false;

    void Start()
    {
        // Hide prompt at start
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Ensure we have an audio source
        if (pumpAudioSource == null)
        {
            pumpAudioSource = GetComponent<AudioSource>();
            if (pumpAudioSource == null)
            {
                pumpAudioSource = gameObject.AddComponent<AudioSource>();
            }
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

        // Only check for interaction if pump hasn't been used yet
        if (!hasBeenUsed)
        {
            // Check if player can interact (either through raycast or proximity)
            bool canInteract = CheckRaycastInteraction() || isPlayerNearby;

            // Show/hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(canInteract);
            }

            // Handle interaction input
            if (canInteract && Input.GetKeyDown(KeyCode.E))
            {
                InteractWithPump();
            }
        }
    }

    private bool CheckRaycastInteraction()
    {
        // Get main camera and perform raycast
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return false;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Visualize the ray in scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        // Check if raycast hits this object
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private void InteractWithPump()
    {
        // Mark the pump as used
        hasBeenUsed = true;

        if (pumpAudioSource != null && !pumpAudioSource.isPlaying)
        {
            // Play the pump sound
            pumpAudioSource.Play();
        }

        // Hide prompt permanently
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    // Trigger zone detection for proximity-based interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenUsed && isInteractionEnabled)
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Make sure to hide the prompt when player leaves the area
            if (interactionPrompt != null && !hasBeenUsed)
                interactionPrompt.SetActive(false);
        }
    }
    // Public method to check if the pump has been used (can be accessed by other scripts)
    public bool HasBeenUsed()
    {
        return hasBeenUsed;
    }
}