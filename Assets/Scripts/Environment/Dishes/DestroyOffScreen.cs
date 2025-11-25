using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float destroyDistance = 7f; // How far below the camera
    [SerializeField] private bool useViewportCheck = true; // Use viewport or distance method
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found!");
        }
    }
    
    void Update()
    {
        if (mainCamera == null) return;
        
        if (useViewportCheck)
        {
            // Method 1: Check if below viewport
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
            
            // Destroy if below the bottom of the screen (with buffer)
            if (viewportPos.y < -0.2f) // -0.2 gives a small buffer below screen
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Method 2: Check distance below camera
            float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
            Debug.Log($"Camera Bottom: {cameraBottom}, Object Y: {transform.position.y}");
            
            if (transform.position.y < cameraBottom - destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}