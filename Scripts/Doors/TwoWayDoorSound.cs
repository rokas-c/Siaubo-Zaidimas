using UnityEngine;

public class TwoWayDoorSound : MonoBehaviour
{
    public AudioSource source;        // Reference to the AudioSource component
    public AudioClip doorSound;       // Sound to play when the door opens
    public HingeJoint hingeJoint;     // Reference to the HingeJoint component
    public float openAngleThreshold = 10f; // Angle to determine if door is "open" in either direction
    public float soundCooldown = 2f; // Delay in seconds before sound can be played again

    private float lastSoundTime = 0f;    // Time when the sound was last played

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>(); // Ensure there's an AudioSource attached

        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>(); // Ensure there's a HingeJoint attached
    }

    void Update()
    {
        // Get the current angle of the door
        float doorAngle = hingeJoint.angle;

        // Check if the door has opened past either -30 or 30 (in both directions)
        if ((doorAngle <= -openAngleThreshold || doorAngle >= openAngleThreshold) && Time.time - lastSoundTime >= soundCooldown)
        {
            PlayDoorSound(); // Play sound when door is opened in either direction
        }
    }

    void PlayDoorSound()
    {
        if (doorSound != null)
        {
            source.PlayOneShot(doorSound); // Play the opening sound
            lastSoundTime = Time.time; // Update the last time the sound was played
        }
    }
}
