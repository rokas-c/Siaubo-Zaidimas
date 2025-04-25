using UnityEngine;

public class PunchInAction : MonoBehaviour
{
    public GameObject PunchInText;
    public AudioClip punchInSound;
    
    // Static variable accessible by any script
    public static bool HasPunchedIn = false;
    
    private bool isPlayerNearby = false;
    
    void Start()
    {
        if (PunchInText != null)
            PunchInText.SetActive(false);
    }
    
    void Update()
    {
        if (isPlayerNearby && !HasPunchedIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Set static flag
                HasPunchedIn = true;
                
                // Play sound
                if (punchInSound != null)
                    AudioSource.PlayClipAtPoint(punchInSound, transform.position);
                
                // Hide text
                PunchInText.SetActive(false);
                
                // Lock all doors when punched in
                LockAllDoors();
            }
        }
    }
    
   private void LockAllDoors()
{
    // Find all door objects with the HingedDoorController script
    HingedDoorController[] allDoors = FindObjectsOfType<HingedDoorController>();
    
    // Lock each door
    foreach (HingedDoorController door in allDoors)
    {
        door.LockDoor(true); // Pass true to indicate locked by punch-in
    }
    
    Debug.Log("All hinged doors have been locked due to punch in!");
}
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!HasPunchedIn)
                PunchInText.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            PunchInText.SetActive(false);
        }
    }
}