using UnityEngine;

public class Walking : MonoBehaviour
{
    public AudioClip walkingSound;
    private AudioSource audioSource;
    
    // Settings
    public float fadeOutSpeed = 2.0f;
    public float walkPitch = 1.0f;
    public float sprintPitch = 1.5f;
    
    private bool wasMoving = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.clip = walkingSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }
    
    void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                       Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
        
        // Start audio when movement begins
        if (isMoving && !wasMoving) {
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
        
        // Update audio when moving
        if (isMoving) {
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, isSprinting ? sprintPitch : walkPitch, Time.deltaTime * 5f);
        }
        // Fade out when stopped
        else if (audioSource.isPlaying) {
            audioSource.volume -= Time.deltaTime * fadeOutSpeed;
            
            if (audioSource.volume <= 0.01f) {
                audioSource.Stop();
                audioSource.volume = 1.0f;
                audioSource.pitch = walkPitch;
            }
        }
        
        wasMoving = isMoving;
    }
}