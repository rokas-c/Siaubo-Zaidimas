using UnityEngine;

public class SmoothZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float normalFOV = 60f;
    public float zoomedFOV = 30f;
    public float zoomSpeed = 10f;

    private Camera cam;
    private float targetFOV;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                enabled = false;
                return;
            }
        }

        targetFOV = normalFOV;
        cam.fieldOfView = normalFOV;
    }

    private void Update()
    {
        // Set target FOV based on right mouse button
        targetFOV = Input.GetMouseButton(1) ? zoomedFOV : normalFOV;

        // Smoothly transition to target FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
}