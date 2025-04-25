using UnityEngine;

public class DoorSound : MonoBehaviour
{
    public AudioSource source;        // Reference to the AudioSource component
    public AudioClip doorSound;       // Sound to play when the door opens
    public AudioClip doorCloseSound;  // Sound to play when the door closes
    public HingeJoint hingeJoint;     // Reference to the HingeJoint component
    public float openAngleThreshold = -30f; // Angle at which sound should play (when door opens)
    public float soundCooldown = 2f; // Delay in seconds before sound can be played again

    private float lastSoundTime = 0f;    // Time when the sound was last played
    private bool hasPlayedSound = false; // Prevent the sound from playing repeatedly
    private HingedDoorController doorController; // Reference to our door controller
    private float lastCheckedAngle = 0f; // To track door movement

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>(); // Ensure there's an AudioSource attached

        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>(); // Ensure there's a HingeJoint attached
            
        // Get the door controller
        doorController = GetComponent<HingedDoorController>();
        
        // Initialize the last checked angle
        lastCheckedAngle = hingeJoint.angle;
    }

    void Update()
    {
        // Skip sound checks if door is locked by punch-in
        if (doorController != null && doorController.isLockedByPunchIn)
            return;
            
        // Get the current angle of the door
        float doorAngle = hingeJoint.angle;

        // If the door has opened past the threshold and hasn't played the sound
        if (doorAngle <= openAngleThreshold && !hasPlayedSound && Time.time - lastSoundTime >= soundCooldown)
        {
            PlayDoorSound(); // Play opening sound when door reaches the open angle threshold
        }

        // Only play closing sound if the door has actually moved to the closed position
        // Check if the door was moving (not at 0) and is now at 0
        if (doorAngle == 0 && lastCheckedAngle != 0 && Time.time - lastSoundTime >= soundCooldown)
        {
            PlayDoorCloseSound(); // Play closing sound when the door is fully closed
        }

        // Reset sound played flag if door closes (if it passes the threshold of opening)
        if (doorAngle > openAngleThreshold)
        {
            hasPlayedSound = false; // Allow opening sound to play again when door is closed
        }
        
        // Update the last checked angle
        lastCheckedAngle = doorAngle;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player enters the trigger zone
        if (other.CompareTag("Player"))
        {
            // Reset hasPlayedSound flag when player enters
            hasPlayedSound = false;
        }
    }

    void PlayDoorSound()
    {
        if (doorSound != null)
        {
            source.PlayOneShot(doorSound); // Play the opening sound
            lastSoundTime = Time.time; // Update the last time the sound was played
            hasPlayedSound = true; // Prevent the sound from playing again until cooldown
        }
    }

    void PlayDoorCloseSound()
    {
        if (doorCloseSound != null)
        {
            source.PlayOneShot(doorCloseSound); // Play the closing sound
            lastSoundTime = Time.time; // Update the last time the sound was played
        }
    }
}