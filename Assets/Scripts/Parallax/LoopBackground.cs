using UnityEngine;

public class ManualLoopingBackground : MonoBehaviour
{
    [Header("Setup")]
    public Transform cameraTransform;   // The camera to follow
    public float backgroundWidth = 10f; // Manually set the width of your sprite in world units
    public float parallaxFactor = 0.5f; // 0 = far, 1 = moves with camera

    private Vector3 lastCameraPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPos = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Move background for parallax effect
        Vector3 camDelta = cameraTransform.position - lastCameraPos;
        transform.position += new Vector3(camDelta.x * parallaxFactor, camDelta.y * parallaxFactor, 0);
        lastCameraPos = cameraTransform.position;

        // Loop background horizontally
        float diff = cameraTransform.position.x - transform.position.x;
        if (diff >= backgroundWidth)
        {
            transform.position += new Vector3(backgroundWidth, 0, 0);
        }
        else if (diff <= -backgroundWidth)
        {
            transform.position -= new Vector3(backgroundWidth, 0, 0);
        }
    }
}
