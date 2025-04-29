using UnityEngine;

public class TwoWayDoorSound : MonoBehaviour
{
    public AudioSource source;                 // Reference to the AudioSource component
    public AudioClip doorSound;                // Sound to play when the door opens
    public HingeJoint hingeJoint;              // Reference to the HingeJoint component
    public float openAngleThreshold = 10f;     // Angle to determine if door is "open" in either direction
    public float soundCooldown = 2f;           // Delay before sound can be played again

    public Transform teleportedObject;         // Object to monitor for movement
    private Vector3 lastTeleportPos;           // Last known position of the object
    private bool isMuted = false;
    private float muteEndTime = 0f;

    private float lastSoundTime = 0f;          // Last time the sound was played

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>();

        if (teleportedObject != null)
            lastTeleportPos = teleportedObject.position;
    }

    void Update()
    {
        // Check for teleported object movement
        if (teleportedObject != null && teleportedObject.position != lastTeleportPos)
        {
            lastTeleportPos = teleportedObject.position;
            isMuted = true;
            muteEndTime = Time.time + 10f; // Mute for 10 seconds
        }

        // Unmute after 10 seconds
        if (isMuted && Time.time >= muteEndTime)
        {
            isMuted = false;
        }

        float doorAngle = hingeJoint.angle;

        // Check door angle and sound cooldown, only play if not muted
        if (!isMuted &&
            (doorAngle <= -openAngleThreshold || doorAngle >= openAngleThreshold) &&
            Time.time - lastSoundTime >= soundCooldown)
        {
            PlayDoorSound();
        }
    }

    void PlayDoorSound()
    {
        if (doorSound != null)
        {
            source.PlayOneShot(doorSound);
            lastSoundTime = Time.time;
        }
    }
}
