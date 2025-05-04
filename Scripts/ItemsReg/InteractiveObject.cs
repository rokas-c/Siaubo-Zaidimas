using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class InteractiveObject : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiPrompt;
    public float maxInteractionDistance = 5f;
    public LayerMask interactableLayers = -1; // Default to everything

    [Header("Interaction Settings")]
    public bool oneTimeInteraction = true;
    public float interactionDelay = 0f;
    public AudioSource interactionSound;

    [Header("Model Disable")]
    public GameObject modelToDisable;

    // Private variables
    private Camera mainCamera;
    private bool isLookingAtObject = false;
    private bool hasInteracted = false;
    private Coroutine delayCoroutine = null;

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

        // Disable UI prompt initially
        if (uiPrompt != null)
            uiPrompt.SetActive(false);

        // Add AudioSource if not already present
        if (interactionSound == null)
            interactionSound = GetComponent<AudioSource>();
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
        if (hasInteracted && oneTimeInteraction)
            return;

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
            if (isLookingAtObject && !hasInteracted)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }
    }

    private void HandleInput()
    {
        if (!isLookingAtObject || hasInteracted)
            return;

        // Check for interaction input
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (delayCoroutine == null)
            {
                delayCoroutine = StartCoroutine(InteractWithDelay());
            }
        }
    }

    private IEnumerator InteractWithDelay()
    {
        // Play sound instantly (if any)
        if (interactionSound != null && interactionSound.clip != null)
        {
            interactionSound.Play();
        }

        // Wait only for the rest of the logic
        if (interactionDelay > 0)
        {
            yield return new WaitForSeconds(interactionDelay);
        }

        // Run the remaining interaction logic
        FinishInteraction();
        delayCoroutine = null;
    }

    private void FinishInteraction()
    {
        if (oneTimeInteraction)
        {
            hasInteracted = true;
            HideUI();
        }

        if (modelToDisable != null)
        {
            modelToDisable.SetActive(false);
        }
    }

    private void ShowUI()
    {
        if (uiPrompt != null)
        {
            uiPrompt.SetActive(true);
        }
    }

    private void HideUI()
    {
        if (uiPrompt != null)
            uiPrompt.SetActive(false);
    }

    // Public method to trigger interaction programmatically
    public void TriggerInteraction()
    {
        if (!hasInteracted || !oneTimeInteraction)
        {
            StartCoroutine(InteractWithDelay());
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
            HideUI();
        }
    }

    // Reset interaction state (useful for save/load systems)
    public void ResetInteraction()
    {
        hasInteracted = false;
    }
}