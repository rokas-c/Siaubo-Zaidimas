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

    // New option to play sound independently from this object
    public bool playSoundIndependently = true;

    [Header("Model Selection")]
    public GameObject[] modelsToEnable;
    public GameObject[] modelsToDisable;


    // Private variables
    private Camera mainCamera;
    private bool isLookingAtObject = false;
    private bool hasInteracted = false;
    private Coroutine delayCoroutine = null;
    private static GameObject soundPlayerObject;

    private void Awake()
    {
        // Ensure the collider has isTrigger set to true
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Disable UI prompt initially
        if (uiPrompt != null)
            uiPrompt.SetActive(false);

        // Add AudioSource if not already present and none is assigned
        if (interactionSound == null)
            interactionSound = GetComponent<AudioSource>();

        // Create global sound player if needed
        EnsureSoundPlayerExists();
    }

    private void EnsureSoundPlayerExists()
    {
        // Create a persistent sound player object if it doesn't exist
        if (soundPlayerObject == null)
        {
            soundPlayerObject = new GameObject("InteractionSoundPlayer");
            DontDestroyOnLoad(soundPlayerObject);
            soundPlayerObject.AddComponent<AudioSource>();
        }
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
        // Play sound before anything else happens
        PlayInteractionSound();

        // Wait only for the rest of the logic
        if (interactionDelay > 0)
        {
            yield return new WaitForSeconds(interactionDelay);
        }

        // Run the remaining interaction logic
        FinishInteraction();
        delayCoroutine = null;
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null && interactionSound.clip != null)
        {
            if (playSoundIndependently)
            {
                // Play the sound on our persistent object
                AudioSource globalSource = soundPlayerObject.GetComponent<AudioSource>();
                globalSource.PlayOneShot(interactionSound.clip, interactionSound.volume);
            }
            else
            {
                // Play on this object's AudioSource
                interactionSound.Play();
            }
        }
    }

    private void FinishInteraction()
    {
        if (oneTimeInteraction)
        {
            hasInteracted = true;
            HideUI();
        }

        ToggleModels();
    }

    private void ToggleModels()
    {
        if (modelsToEnable != null)
        {
            foreach (GameObject model in modelsToEnable)
            {
                if (model != null)
                {
                    model.SetActive(true);
                }
            }
        }

        // Disable all target models
        if (modelsToDisable != null)
        {
            foreach (GameObject model in modelsToDisable)
            {
                if (model != null)
                {
                    model.SetActive(false);
                }
            }
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