using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class ReflectnScale : MonoBehaviour
{
    [SerializeField, Range(0f, 90f)] private float minRotationDeg = 1.5f;
    [SerializeField, Range(0f, 90f)] private float maxRotationDeg = 90f;
    [SerializeField] bool invert = false;

    [Header("Depth Settings")]
    [SerializeField] private float closestZ = -10f;  // When at center
    [SerializeField] private float furthestZ = 900f; // When at edge
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Convert world position to viewport position (0 to 1 range)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // viewportPos.x: 0 = left edge, 0.5 = center, 1 = right edge
        // Convert to distance from center: 0 = center, 1 = edge
        float distanceFromCenter = Mathf.Abs(viewportPos.x - 0.5f) * 2f;

        // Clamp to prevent going beyond screen bounds
        float extendRange = 0.2f; // how far past edges (in viewport units) to still count, e.g. 20% of screen width
        distanceFromCenter = Mathf.InverseLerp(0f, 1f + extendRange, distanceFromCenter);
        distanceFromCenter = Mathf.Clamp01(distanceFromCenter);

        // Lerp between min and max rotation based on distance
        float targetRotation = Mathf.Lerp(minRotationDeg, maxRotationDeg, distanceFromCenter);

        // Determine rotation direction based on which side of screen
        bool leftSide = viewportPos.x < 0.5f;
        if (invert) leftSide = !leftSide;
        if (leftSide)
        {
            // Left side - rotate counterclockwise from 90
            targetRotation = 90f - targetRotation;
        }
        else
        {
            // Right side - rotate clockwise from 90
            targetRotation = 90f + targetRotation;
        }

        // Log the values
        // Debug.Log($"Viewport X: {viewportPos.x:F3}, Distance from center: {distanceFromCenter:F3}, Target rotation: {targetRotation:F2}");

        float targetZ = Mathf.Lerp(closestZ, furthestZ, distanceFromCenter);
        Vector3 newPos = transform.position;
        newPos.z = targetZ;
        transform.position = newPos;
        // Apply Y-axis rotation
        transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
    }
}