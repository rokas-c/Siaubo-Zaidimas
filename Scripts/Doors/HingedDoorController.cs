using UnityEngine;

public class HingedDoorController : MonoBehaviour
{
    public bool startLocked = false;
    public bool isLockedByPunchIn = false;
    
    private HingeJoint hingeJoint;
    private JointLimits originalLimits;
    
    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        
        if (hingeJoint == null)
            return;
        
        originalLimits = hingeJoint.limits;
        
        if (startLocked)
        {
            LockDoor();
        }
    }
    
    public void LockDoor(bool isPunchIn = false)
{
    if (hingeJoint != null)
    {
        JointLimits frozenLimits = new JointLimits();
        float currentAngle = hingeJoint.angle;
        frozenLimits.min = currentAngle;
        frozenLimits.max = currentAngle;
        
        hingeJoint.limits = frozenLimits;
        hingeJoint.useLimits = true;
        
        // Set the flag if locked by punch-in.
        isLockedByPunchIn = isPunchIn;
    }
}
    
    public void UnlockDoor()
    {
        if (hingeJoint != null)
        {
            hingeJoint.limits = originalLimits;
        }
    }
}