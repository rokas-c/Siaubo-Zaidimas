using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ObjectInfo : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject primaryUIPrompt;
    public GameObject secondaryUIPrompt;
    public float secondaryUIDisplayTime = 3f;
    public float maxInteractionDistance = 5f;
    public LayerMask interactableLayers = -1; // Default to everything

    // Private variables
    private Camera mainCamera;
    private bool isLookingAtObject = false;
    private GameObject activeUI = null;
    private bool secondaryUIActive = false;
    private Coroutine uiTimerCoroutine = null;

    private void Awake()
    {
        // Ensure the collider has isTrigger set to true
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("InteractiveObject requires a Collider component!");
        }

        // Disable UI prompts initially
        if (primaryUIPrompt != null)
            primaryUIPrompt.SetActive(false);

        if (secondaryUIPrompt != null)
            secondaryUIPrompt.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene!");
        }
    }

    private void Update()
    {
        CheckIfLookingAtObject();
        HandleInput();
    }

    private void CheckIfLookingAtObject()
    {
        bool wasLookingAtObject = isLookingAtObject;
        isLookingAtObject = false;

        if (mainCamera == null)
            return;

        // Simple raycast from center of screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Only detect if directly looking at the collider
        if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactableLayers))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isLookingAtObject = true;
            }
        }

        // Handle UI visibility changes
        if (isLookingAtObject != wasLookingAtObject)
        {
            if (isLookingAtObject)
            {
                ShowPrimaryUI();
            }
            else
            {
                HideAllUI();
            }
        }
    }

    private void HandleInput()
    {
        if (!isLookingAtObject)
            return;

        // Check for interaction input (only for the first UI)
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && !secondaryUIActive)
        {
            Interact();
        }
    }

    private void Interact()
    {
        // Show secondary UI with auto-return timer
        ShowSecondaryUI();
    }

    private void ShowPrimaryUI()
    {
        if (primaryUIPrompt != null)
        {
            primaryUIPrompt.SetActive(true);

            if (secondaryUIPrompt != null)
                secondaryUIPrompt.SetActive(false);

            activeUI = primaryUIPrompt;
            secondaryUIActive = false;
        }
    }

    private void ShowSecondaryUI()
    {
        if (secondaryUIPrompt != null)
        {
            secondaryUIPrompt.SetActive(true);

            if (primaryUIPrompt != null)
                primaryUIPrompt.SetActive(false);

            activeUI = secondaryUIPrompt;
            secondaryUIActive = true;

            // Cancel any existing timer
            if (uiTimerCoroutine != null)
            {
                StopCoroutine(uiTimerCoroutine);
            }

            // Start timer to return to primary UI
            uiTimerCoroutine = StartCoroutine(ReturnToPrimaryUIAfterDelay(secondaryUIDisplayTime));
        }
    }

    private IEnumerator ReturnToPrimaryUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isLookingAtObject) // Only switch if still looking at object
        {
            ShowPrimaryUI();
        }

        uiTimerCoroutine = null;
    }

    private void HideAllUI()
    {
        if (primaryUIPrompt != null)
            primaryUIPrompt.SetActive(false);

        if (secondaryUIPrompt != null)
            secondaryUIPrompt.SetActive(false);

        activeUI = null;
        secondaryUIActive = false;
    }

    // Public method to trigger interaction programmatically
    public void TriggerInteraction()
    {
        if (isLookingAtObject)
        {
            Interact();
        }
    }

    // Optional: Allow trigger-based interactions too
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Only show UI if the player is looking at the object
            CheckIfLookingAtObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideAllUI();
        }
    }
}