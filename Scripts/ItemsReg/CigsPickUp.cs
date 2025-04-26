using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigsPickUp : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject cigsInHandObject; // The cigarettes to show in player's hand
    [SerializeField] private GameObject pickupPromptText; // UI text prompt for pickup

    private bool playerInRange = false;
    private bool isPickedUp = false;

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
    }

    void Update()
    {
        // Only show prompt and allow pickup when player is in range and item isn't picked up yet
        if (playerInRange && !isPickedUp)
        {
            // Show pickup prompt
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(true);
            }

            // Check for E key press to pick up
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpCigs();
            }
        }
        else if (pickupPromptText != null)
        {
            // Hide prompt if player leaves range or item is picked up
            pickupPromptText.SetActive(false);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if player left trigger zone
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Hide the prompt when player leaves
            if (pickupPromptText != null)
            {
                pickupPromptText.SetActive(false);
            }
        }
    }
}