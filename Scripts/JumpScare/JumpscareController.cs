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

    [Header("Slide Settings")]
    public bool useSlideEffect = true;
    public Transform slideStartPosition;
    public Transform slideEndPosition;
    public float slideSpeed = 4f;
    public bool maintainRotation = true;

    [Header("Darkness Effect")]
    public bool useDarknessEffect = true;
    [Range(0f, 1f)] public float darknessFactor = 0.2f;
    public float brightnessTransitionSpeed = 3f;

    public Transform teleportLocation; // Where to teleport the player

    public Transform scaryModelTeleportLocation; // Where to teleport the jumpscare model

    public Collider colliderToEnable; // Collider to enable during jumpscare

    private AudioSource audioSource;
    private PlayerMovement playerMovement;
    private Camera playerCamera;
    private float originalFOV;
    private Quaternion originalCameraRotation;
    private Quaternion originalPlayerRotation;

    // Store the original position of the scary model
    private Vector3 originalModelPosition;
    private Quaternion originalModelRotation;

    // For tracking original materials and renderers
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, Material[]> darkMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> modelRenderers = new List<Renderer>();

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

        // Store original model position and rotation if available
        if (scaryModel != null)
        {
            originalModelPosition = scaryModel.transform.position;
            originalModelRotation = scaryModel.transform.rotation;

            // Cache all renderers and their materials
            if (useDarknessEffect)
            {
                CacheModelMaterials();
            }
        }
    }

    private void CacheModelMaterials()
    {
        // Get all renderers from the model and its children
        Renderer[] renderers = scaryModel.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Skip particle systems and trail renderers
            if (renderer is ParticleSystemRenderer || renderer is TrailRenderer)
                continue;

            modelRenderers.Add(renderer);

            // Store original materials
            Material[] originalMats = renderer.materials;
            originalMaterials[renderer] = originalMats;

            // Create dark versions of materials
            Material[] darkMats = new Material[originalMats.Length];
            for (int i = 0; i < originalMats.Length; i++)
            {
                darkMats[i] = new Material(originalMats[i]);
                Color darkColor = darkMats[i].color * darknessFactor;
                darkColor.a = darkMats[i].color.a; // Preserve alpha
                darkMats[i].color = darkColor;

                // Also darken emission if present
                if (darkMats[i].HasProperty("_EmissionColor"))
                {
                    Color emissionColor = darkMats[i].GetColor("_EmissionColor");
                    darkMats[i].SetColor("_EmissionColor", emissionColor * darknessFactor);
                }
            }

            darkMaterials[renderer] = darkMats;
        }
    }

    private void SetModelDarkness(bool dark)
    {
        if (!useDarknessEffect || modelRenderers.Count == 0)
            return;

        foreach (Renderer renderer in modelRenderers)
        {
            if (renderer == null)
                continue;

            renderer.materials = dark ? darkMaterials[renderer] : originalMaterials[renderer];
        }
    }

    private IEnumerator FadeModelToBrightness(float duration)
    {
        if (!useDarknessEffect || modelRenderers.Count == 0)
            yield break;

        float elapsedTime = 0f;

        // Create a temporary set of materials to animate
        Dictionary<Renderer, Material[]> tempMaterials = new Dictionary<Renderer, Material[]>();

        // Initialize with dark materials
        foreach (Renderer renderer in modelRenderers)
        {
            if (renderer == null)
                continue;

            Material[] darkMats = darkMaterials[renderer];
            Material[] tempMats = new Material[darkMats.Length];

            for (int i = 0; i < darkMats.Length; i++)
            {
                tempMats[i] = new Material(darkMats[i]);
            }

            tempMaterials[renderer] = tempMats;
            renderer.materials = tempMats;
        }

        // Animate to original brightness
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            foreach (Renderer renderer in modelRenderers)
            {
                if (renderer == null)
                    continue;

                Material[] origMats = originalMaterials[renderer];
                Material[] tempMats = tempMaterials[renderer];

                for (int i = 0; i < tempMats.Length; i++)
                {
                    // Lerp color
                    Color origColor = origMats[i].color;
                    Color darkColor = darkMaterials[renderer][i].color;
                    tempMats[i].color = Color.Lerp(darkColor, origColor, t);

                    // Lerp emission if present
                    if (tempMats[i].HasProperty("_EmissionColor") && origMats[i].HasProperty("_EmissionColor"))
                    {
                        Color origEmission = origMats[i].GetColor("_EmissionColor");
                        Color darkEmission = darkMaterials[renderer][i].GetColor("_EmissionColor");
                        tempMats[i].SetColor("_EmissionColor", Color.Lerp(darkEmission, origEmission, t));
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set back to original materials
        foreach (Renderer renderer in modelRenderers)
        {
            if (renderer == null)
                continue;

            renderer.materials = originalMaterials[renderer];
        }

        // Clean up temporary materials
        foreach (Material[] mats in tempMaterials.Values)
        {
            foreach (Material mat in mats)
            {
                Destroy(mat);
            }
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

        // Make model dark if using darkness effect
        if (useDarknessEffect)
        {
            SetModelDarkness(true);
        }

        // Position the scary model if using slide effect
        if (useSlideEffect && scaryModel != null && slideStartPosition != null && slideEndPosition != null)
        {
            // Store original position and rotation
            Vector3 originalPos = scaryModel.transform.position;
            Quaternion originalRot = scaryModel.transform.rotation;

            // Position the model at start position
            scaryModel.transform.position = slideStartPosition.position;

            // Keep original rotation if maintainRotation is true, otherwise use the slide start rotation
            if (!maintainRotation)
            {
                scaryModel.transform.rotation = slideStartPosition.rotation;
            }
            else
            {
                scaryModel.transform.rotation = originalRot;
            }

            // Slide the model into view along the predefined path
            float slideTime = 0f;
            while (slideTime < 1.0f)
            {
                slideTime += Time.deltaTime * slideSpeed;
                scaryModel.transform.position = Vector3.Lerp(slideStartPosition.position, slideEndPosition.position, Mathf.SmoothStep(0, 1, slideTime));
                yield return null;
            }

            // Ensure the model reaches the exact end position
            scaryModel.transform.position = slideEndPosition.position;

            // Start fading back to normal brightness
            if (useDarknessEffect)
            {
                StartCoroutine(FadeModelToBrightness(1f / brightnessTransitionSpeed));
            }
        }
        else if (useDarknessEffect)
        {
            // If not using slide but still using darkness, fade back to normal brightness
            StartCoroutine(FadeModelToBrightness(1f / brightnessTransitionSpeed));
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

        // Make sure the model is back to normal brightness for later
        if (useDarknessEffect)
        {
            SetModelDarkness(false);
        }

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

    // Clean up materials when destroyed
    private void OnDestroy()
    {
        if (useDarknessEffect)
        {
            foreach (Material[] mats in darkMaterials.Values)
            {
                foreach (Material mat in mats)
                {
                    Destroy(mat);
                }
            }
        }
    }
}