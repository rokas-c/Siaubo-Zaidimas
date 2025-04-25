using UnityEngine;

public class HingedDoorController : MonoBehaviour
{
    public bool startLocked = false;
    public bool isLockedByPunchIn = false;
    
    private HingeJoint hingeJoint;
    private Rigidbody doorRigidbody;
    private bool wasKinematic; // Store original kinematic state
    
    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        doorRigidbody = GetComponent<Rigidbody>();
        
        if (doorRigidbody == null)
            return;
            
        // Store the original kinematic state
        wasKinematic = doorRigidbody.isKinematic;
        
        if (startLocked)
        {
            LockDoor();
        }
    }
    
    public void LockDoor(bool isPunchIn = false)
    {
        if (doorRigidbody != null)
        {
            // Simply make the door kinematic (static)
            doorRigidbody.linearVelocity = Vector3.zero;
            doorRigidbody.angularVelocity = Vector3.zero;
            doorRigidbody.isKinematic = true;
            
            // Set the flag if locked by punch-in
            isLockedByPunchIn = isPunchIn;
        }
    }
    
    public void UnlockDoor()
    {
        if (doorRigidbody != null)
        {
            // Return to the original kinematic state
            doorRigidbody.isKinematic = wasKinematic;
        }
    }
}