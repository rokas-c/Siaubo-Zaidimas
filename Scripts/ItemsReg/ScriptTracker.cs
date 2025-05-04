using UnityEngine;

public class ObjectToggleTracker : MonoBehaviour
{
    [Header("Tracked Object")]
    public GameObject trackedObject; // The object you want to track for visibility

    [Header("Objects To Toggle")]
    public GameObject willBeHidden; // This object is disabled when tracked object is active
    public GameObject willBeActive; // This object is enabled when tracked object is active

    private bool wasActiveLastFrame = false;

    void Update()
    {
        bool isActiveNow = trackedObject != null && trackedObject.activeSelf;

        if (isActiveNow && !wasActiveLastFrame)
        {
            // Object just became active
            if (willBeHidden != null) willBeHidden.SetActive(false);
            if (willBeActive != null) willBeActive.SetActive(true);
        }
        else if (!isActiveNow && wasActiveLastFrame)
        {
            // Object just became inactive
            if (willBeHidden != null) willBeHidden.SetActive(true);
            if (willBeActive != null) willBeActive.SetActive(false);
        }

        wasActiveLastFrame = isActiveNow;
    }
}