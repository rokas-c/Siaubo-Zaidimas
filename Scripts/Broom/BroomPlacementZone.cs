using UnityEngine;
using UnityEngine.UI;

public class BroomPlacementZone : MonoBehaviour
{
    public GameObject broomInWorldObject; // The broom object in the world
    public GameObject broomOnPlayerObject; // The broom on the player
    public GameObject placementPromptText; // UI text to show when player can place the broom
    public Transform broomFinalPosition; // Where to place the broom when returned
    
    private bool playerInZone = false;
    private bool allPuddlesCleaned = false;
    private bool broomPlaced = false;
    
    void Start()
    {
        // Hide the placement prompt initially
        if (placementPromptText != null)
            placementPromptText.SetActive(false);
            
        // Find all puddles at start
        RefreshPuddleCount();
    }
    
    void Update()
    {
        // Check if all puddles are cleaned
        if (!allPuddlesCleaned)
        {
            CheckPuddleStatus();
        }
        
        // Only allow placement if player is in zone, all puddles cleaned, and broom not yet placed
        if (playerInZone && allPuddlesCleaned && !broomPlaced)
        {
            // Show placement prompt
            if (placementPromptText != null)
                placementPromptText.SetActive(true);
                
            // Check for key press to place broom
            if (Input.GetKeyDown(KeyCode.E) && broomOnPlayerObject.activeInHierarchy)
            {
                PlaceBroom();
            }
        }
        else if (placementPromptText != null)
        {
            placementPromptText.SetActive(false);
        }
    }
    
    void CheckPuddleStatus()
    {
        // Count remaining puddles
        GameObject[] remainingPuddles = GameObject.FindGameObjectsWithTag("Puddle");
        
        // If no puddles remain, all are cleaned
        if (remainingPuddles.Length == 0)
        {
            allPuddlesCleaned = true;
            
            // Notify the GameManager if it exists
            if (GameManager.Instance != null)
            {
                // This assumes GameManager has an event for this
                // If not, you can safely remove this
                GameManager.Instance.onAllPuddlesCleaned.Invoke();
            }
        }
    }
    
    void RefreshPuddleCount()
    {
        GameObject[] puddles = GameObject.FindGameObjectsWithTag("Puddle");
        
        // Reset the cleaned status if any puddles exist
        allPuddlesCleaned = (puddles.Length == 0);
    }
    
    void PlaceBroom()
    {
        
        // Hide the broom on player
        broomOnPlayerObject.SetActive(false);
        
        // Show and position the world broom
        if (broomInWorldObject != null)
        {
            broomInWorldObject.SetActive(true);
            
            // If we have a final position transform, use it
            if (broomFinalPosition != null)
            {
                broomInWorldObject.transform.position = broomFinalPosition.position;
                broomInWorldObject.transform.rotation = broomFinalPosition.rotation;
            }
            
            // Permanently disable colliders on the broom to prevent picking it up again
            Collider[] colliders = broomInWorldObject.GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            
            // Disable any scripts that might allow interaction
            BroomAction broomAction = broomInWorldObject.GetComponent<BroomAction>();
            if (broomAction != null)
            {
                broomAction.enabled = false;
            }
        }
        
        // Mark as placed
        broomPlaced = true;
        
        // Hide the prompt
        if (placementPromptText != null)
            placementPromptText.SetActive(false);
            
        // Notify GameManager if it exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BroomTaskCompleted();
        }
    }
    
    // Trigger detection for player entering/exiting the zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            
            // Hide the prompt when player leaves
            if (placementPromptText != null)
                placementPromptText.SetActive(false);
        }
    }
}