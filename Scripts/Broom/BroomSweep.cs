using UnityEngine;

public class BroomSweep : MonoBehaviour
{
    public AudioClip sweepSound;
    public float cooldownTime = 4f;
    
    private AudioSource audioSource;
    private bool canSweep = true;
    private float cooldownTimer = 0f;
    
    void Start()
    {
        // Add an AudioSource component if one doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        // Handle cooldown timer
        if (!canSweep)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canSweep = true;
            }
        }
        
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0) && canSweep)
        {
            // Print "sweep" to console
            Debug.Log("sweep");
            
            // Play sweep sound if available
            if (sweepSound != null)
            {
                audioSource.PlayOneShot(sweepSound);
            }
            
            // Start cooldown
            canSweep = false;
            cooldownTimer = cooldownTime;
        }
    }
}