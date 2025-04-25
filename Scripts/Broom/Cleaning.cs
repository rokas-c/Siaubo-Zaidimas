using UnityEngine;
using System.Collections;

public class Cleaning : MonoBehaviour
{
    private GameObject broom;
    public float fadeOutTime = 5f; // How long the fadeout takes in seconds
    
    private SpriteRenderer spriteRenderer;
    private Collider2D trashCollider;
    private bool isFading = false;
    
    void Start()
    {
        // Find the broom object in the scene
        broom = GameObject.FindWithTag("Broom");
        
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        // Get the collider for disabling during fade out
        trashCollider = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        if (IsBroomPickedUp() && !isFading)
        {
            // Start fade out instead of immediate destruction
            StartCoroutine(FadeOutAndDestroy());
        }
        else if (!isFading)
        {
        }
    }
    
    bool IsBroomPickedUp()
    {
        // First check if we have found the broom
        if (broom == null)
        {
            broom = GameObject.FindWithTag("Broom");
            if (broom == null) return false;
        }
        
        // Check if the broom is active in the hierarchy (picked up)
        if (broom.activeInHierarchy)
        {
            // Get the BroomSweep component to check if it's currently sweeping
            BroomSweep broomSweep = broom.GetComponent<BroomSweep>();
            if (broomSweep != null && broomSweep.canSweep)
            {
                // Player has the broom and can sweep
                return true;
            }
        }
        
        return false;
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        
        // Disable the collider so it can't be clicked again
        if (trashCollider != null)
        {
            trashCollider.enabled = false;
        }
        
        // Make sure we have something to fade
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            float elapsedTime = 0f;
            
            // Gradually reduce alpha over time
            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeOutTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
                yield return null;
            }
            
            // Make sure alpha is zero
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
        
        // Finally destroy the object after fading
        Destroy(gameObject);
    }
}