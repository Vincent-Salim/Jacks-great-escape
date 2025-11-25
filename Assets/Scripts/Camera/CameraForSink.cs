using UnityEngine;

/// <summary>
/// Simple vertical camera follow for sink.
/// - Follows the target vertically only, keeping Jack near the bottom of the screen.
/// </summary>
[ExecuteAlways]
public class VerticalCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset & Viewport")]
    [Tooltip("How far from bottom of screen to place the target (in world units).")]
    public float bottomMargin = 2f;

    [Header("Smoothing - how quickly the camera follows")]
    public float smoothTime = 0.12f;
    private float velocityY = 0f;

    [Header("Vertical bounds")]
    public float minY = -Mathf.Infinity;
    public float maxY = Mathf.Infinity;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main ?? GetComponent<Camera>();
        if (target == null && PlayerController.Instance != null)
            target = PlayerController.Instance.transform;
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // Compute desired camera Y so target sits at bottom + small margin at boottom
        float halfHeight = cam.orthographicSize;
        float desiredY = target.position.y + halfHeight - bottomMargin;

        // Clamp
        desiredY = Mathf.Clamp(desiredY, minY, maxY);

        // Smoothly move camera vertically
        float newY = Mathf.SmoothDamp(transform.position.y, desiredY, ref velocityY, smoothTime);

        // Keep X and Z fixed
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
