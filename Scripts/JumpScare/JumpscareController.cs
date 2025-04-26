using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI elements like the fade panel

public class JumpscareController : MonoBehaviour
{
    public GameObject scaryModel; // The object to focus on
    public float jumpscareLength = 3f; // How long the jumpscare lasts
    public AudioClip jumpscareSound; // Optional sound effect
    public float cameraZoomFOV = 40f; // FOV during jumpscare (lower = more zoomed in)
    public float fadeSpeed = 1.5f; // Speed of the screen fade
    
    [Tooltip("Location to teleport the player after jumpscare")]
    [SerializeField] private Transform teleportLocation; // Where to teleport the player
    
    [Tooltip("Location to teleport the jumpscare model to")]
    [SerializeField] private Transform scaryModelTeleportLocation; // Where to teleport the jumpscare model
    
    [Tooltip("Collider to enable during jumpscare")]
    [SerializeField] private Collider colliderToEnable; // Collider to enable during jumpscare
    
    private AudioSource audioSource;
    private PlayerMovement playerMovement;
    private Camera playerCamera;
    private float originalFOV;
    private Quaternion originalCameraRotation;
    private Quaternion originalPlayerRotation;
    
    // UI elements for fading
    private Image fadePanel;
    
    private void Start()
    {
        // Add audio source component if sound effect is provided
        if (jumpscareSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = jumpscareSound;
            audioSource.playOnAwake = false;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Create fade panel if it doesn't exist
        CreateFadePanel();
        
        // Make sure the collider is disabled at start if assigned
        if (colliderToEnable != null)
        {
            colliderToEnable.enabled = false;
        }
    }
    
    private void CreateFadePanel()
    {
        // Check if Canvas exists in the scene, if not create one
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FadeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Check if fade panel exists, if not create one
        Transform fadePanelTransform = canvas.transform.Find("FadePanel");
        if (fadePanelTransform == null)
        {
            GameObject fadePanelObj = new GameObject("FadePanel");
            fadePanelObj.transform.SetParent(canvas.transform, false);
            
            fadePanel = fadePanelObj.AddComponent<Image>();
            fadePanel.color = new Color(0, 0, 0, 0); // Start transparent
            
            // Set panel to cover the entire screen
            RectTransform rt = fadePanel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            fadePanel = fadePanelTransform.GetComponent<Image>();
        }
        
        // Make sure the panel starts invisible
        fadePanel.color = new Color(0, 0, 0, 0);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Get references to player components
            playerMovement = other.GetComponent<PlayerMovement>();
            
            if (playerMovement != null)
            {
                playerCamera = playerMovement.playerCamera;
                
                if (playerCamera != null && scaryModel != null && fadePanel != null)
                {
                    // Store original state
                    originalFOV = playerCamera.fieldOfView;
                    originalCameraRotation = playerCamera.transform.rotation;
                    originalPlayerRotation = other.transform.rotation;
                    
                    // Start jumpscare coroutine
                    StartCoroutine(TriggerJumpscare(other.gameObject));
                }
            }
        }
    }
    
    private IEnumerator TriggerJumpscare(GameObject player)
    {
        // Disable player movement
        playerMovement.enabled = false;
        
        // Enable the collider if assigned
        if (colliderToEnable != null)
        {
            colliderToEnable.enabled = true;
        }
        
        // Play sound effect if available
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.Play();
        }
        
        // Duration of the jumpscare
        float elapsedTime = 0f;
        
        while (elapsedTime < jumpscareLength)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculate direction to scary model
            Vector3 directionToTarget = scaryModel.transform.position - playerCamera.transform.position;
            
            // Smoothly rotate camera to look at the scary model
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            
            // Apply rotation to both player body and camera
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, 
                                                        Quaternion.Euler(0, targetRotation.eulerAngles.y, 0), 
                                                        Time.deltaTime * 5f);
                                                        
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, 
                                                              Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0), 
                                                              Time.deltaTime * 5f);
            
            // Zoom in camera
            playerCamera.fieldOfView = Mathf.Lerp(originalFOV, cameraZoomFOV, elapsedTime / (jumpscareLength * 0.3f));
            
            yield return null;
        }
        
        // Wait a moment at the peak of the jumpscare
        yield return new WaitForSeconds(0.5f);
        
        // Fade to black
        float fadeInTime = 0f;
        
        while (fadeInTime < 1f)
        {
            fadeInTime += Time.deltaTime * fadeSpeed;
            fadePanel.color = new Color(0, 0, 0, Mathf.Clamp01(fadeInTime));
            yield return null;
        }
        
        // Ensure the screen is completely black
        fadePanel.color = new Color(0, 0, 0, 1);
        
        // Wait briefly while screen is black
        yield return new WaitForSeconds(0.5f);
        
        // Teleport the jumpscare model if a teleport location is specified
        if (scaryModelTeleportLocation != null && scaryModel != null)
        {
            // Check if the scary model has a character controller
            CharacterController modelController = scaryModel.GetComponent<CharacterController>();
            if (modelController != null)
            {
                modelController.enabled = false;
                scaryModel.transform.position = scaryModelTeleportLocation.position;
                scaryModel.transform.rotation = scaryModelTeleportLocation.rotation;
                modelController.enabled = true;
            }
            else
            {
                // If no character controller, just teleport directly
                scaryModel.transform.position = scaryModelTeleportLocation.position;
                scaryModel.transform.rotation = scaryModelTeleportLocation.rotation;
            }
        }
        
        // Teleport the player if a teleport location is specified
        if (teleportLocation != null)
        {
            CharacterController charController = player.GetComponent<CharacterController>();
            
            // Disable character controller before teleporting to avoid physics issues
            if (charController != null)
            {
                charController.enabled = false;
                player.transform.position = teleportLocation.position;
                player.transform.rotation = teleportLocation.rotation;
                charController.enabled = true;
            }
            else
            {
                // If no character controller, just teleport directly
                player.transform.position = teleportLocation.position;
                player.transform.rotation = teleportLocation.rotation;
            }
            
            // Reset camera rotation to match new player orientation
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = originalFOV;
            }
        }
        
        // Fade back in
        float fadeOutTime = 1f;
        
        while (fadeOutTime > 0f)
        {
            fadeOutTime -= Time.deltaTime * fadeSpeed;
            fadePanel.color = new Color(0, 0, 0, Mathf.Clamp01(fadeOutTime));
            yield return null;
        }
        
        // Ensure the screen is completely transparent
        fadePanel.color = new Color(0, 0, 0, 0);
        
        // Re-enable player movement
        playerMovement.enabled = true;
        
        // Disable the trigger so it doesn't happen again
        gameObject.GetComponent<Collider>().enabled = false;
    }
}