using UnityEngine;

public class CigsPlacementArea : MonoBehaviour
{
    [Header("Placement Settings")]
    public GameObject placementPromptText;
    public GameObject cigsOnPlayer;
    public float interactionDistance = 5f;
    public Collider placementCollider;

    public AudioClip VoiceActing;

    [Header("Events")]
    public FuelPumpInteraction connectedFuelPump;

    private Camera playerCamera;
    private CigsPickUp cigsPickUpScript;
    private bool isLookingAtPlacementArea = false;

    [HideInInspector]
    public bool cigsPlaced = false;

    void Start()
    {
        playerCamera = Camera.main;
        cigsPickUpScript = FindObjectOfType<CigsPickUp>();

        // Use this object's collider if none specified
        if (placementCollider == null)
            placementCollider = GetComponent<Collider>();

        // Hide the placement prompt initially
        if (placementPromptText != null)
            placementPromptText.SetActive(false);

        // Find fuel pumps in the scene if not set
        if (connectedFuelPump == null)
            connectedFuelPump = FindObjectOfType<FuelPumpInteraction>();
    }

    void Update()
    {
        // Skip if cigs already placed or player not holding them
        if (cigsPlaced || !IsHoldingCigs())
        {
            if (placementPromptText != null && placementPromptText.activeInHierarchy)
                placementPromptText.SetActive(false);
            return;
        }

        // Check if looking at placement area
        isLookingAtPlacementArea = CheckIfLookingAtPlacementArea();

        // Update prompt visibility
        if (placementPromptText != null)
            placementPromptText.SetActive(isLookingAtPlacementArea);

        // Handle placement input
        if (isLookingAtPlacementArea && Input.GetKeyDown(KeyCode.Mouse0))
            PlaceCigs();
    }

    private bool IsHoldingCigs()
    {
        return cigsOnPlayer != null && cigsOnPlayer.activeInHierarchy;
    }

    private bool CheckIfLookingAtPlacementArea()
    {
        if (placementCollider == null || playerCamera == null)
            return false;

        // Cast a ray from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Check direct hit first
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance) && hit.collider == placementCollider)
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            return true;
        }

        // Additional check with small offsets for better user experience
        float offset = 0.1f;
        Vector3[] checkPoints = new Vector3[]
        {
            new Vector3(0.5f + offset, 0.5f, 0),  // Right
            new Vector3(0.5f - offset, 0.5f, 0),  // Left
            new Vector3(0.5f, 0.5f + offset, 0),  // Up
            new Vector3(0.5f, 0.5f - offset, 0)   // Down
        };

        foreach (Vector3 checkPoint in checkPoints)
        {
            Ray offsetRay = playerCamera.ViewportPointToRay(checkPoint);
            if (Physics.Raycast(offsetRay, out hit, interactionDistance) && hit.collider == placementCollider)
            {
                Debug.DrawLine(offsetRay.origin, hit.point, Color.yellow);
                return true;
            }
        }

        return false;
    }

    private void PlaceCigs()
    {
        if (!IsHoldingCigs())
            return;

        // Hide the cigarettes on player
        if (cigsOnPlayer != null)
            cigsOnPlayer.SetActive(false);

        // Update the pickup script if needed
        if (cigsPickUpScript != null)
        {
            // Try direct method call
            var placementMethod = cigsPickUpScript.GetType().GetMethod("OnCigsPlaced");
            if (placementMethod != null)
            {
                placementMethod.Invoke(cigsPickUpScript, null);
            }
            else
            {
                // Fallback to field update
                var field = cigsPickUpScript.GetType().GetField("isPickedUp");
                if (field != null)
                    field.SetValue(cigsPickUpScript, false);
            }
        }

        // Hide the prompt
        if (placementPromptText != null)
            placementPromptText.SetActive(false);

        // Set flag that cigs have been placed
        cigsPlaced = true;

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && VoiceActing != null)
        {
            audioSource.clip = VoiceActing;
            audioSource.Play();
        }

    }
}