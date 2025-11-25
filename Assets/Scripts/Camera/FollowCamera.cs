using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform cameraTransform;
    public float zOffset = 10f; 
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    
    void LateUpdate()
    {
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            cameraTransform.position.z + zOffset
        );
    }
}