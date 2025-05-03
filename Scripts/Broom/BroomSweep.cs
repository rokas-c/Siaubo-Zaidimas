using UnityEngine;

public class BroomSweep : MonoBehaviour
{
    public AudioClip sweepSound;
    public float cooldownTime = 4f;

    private AudioSource audioSource;
    public bool canSweep = true; // Changed to public
    private float cooldownTimer = 0f;
    private bool isCurrentlySweeping = false; // New variable to track sweep action

    // Add this public method to check sweep status
    public bool IsActivelySweepping()
    {
        return isCurrentlySweeping;
    }

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

        // Update whether we're actively sweeping
        isCurrentlySweeping = Input.GetMouseButton(0) && !canSweep;

        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0) && canSweep)
        {

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

    void OnCollisionEnter2D(Collision2D collision) // Changed from OnTriggerEnter2D
    {
        Debug.Log("Broom collided with: " + collision.gameObject.name);
    }
}