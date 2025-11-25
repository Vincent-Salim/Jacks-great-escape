using UnityEngine;
using UnityEngine.UI;

public class DisplayCameraOutput : MonoBehaviour
{
    public Camera renderCamera;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (renderCamera != null && renderCamera.targetTexture != null)
        {
            rawImage.texture = renderCamera.targetTexture;
        }
    }
}