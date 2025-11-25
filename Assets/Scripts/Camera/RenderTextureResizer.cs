using UnityEngine;

public class RenderTextureResizer : MonoBehaviour
{
    private Camera cam;
    private RenderTexture dynamicRT;
    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateRenderTexture();
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateRenderTexture();
        }
    }

    void UpdateRenderTexture()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        // Clean up old render texture
        if (dynamicRT != null)
        {
            dynamicRT.Release();
            Destroy(dynamicRT);
        }

        // Create new render texture with screen dimensions
        dynamicRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        dynamicRT.name = "Dynamic PostProcessing RT";
        dynamicRT.antiAliasing = 1;
        
        // Assign to camera
        cam.targetTexture = dynamicRT;
        
        Debug.Log($"Render Texture resized to: {Screen.width}x{Screen.height}");
    }

    void OnDestroy()
    {
        if (dynamicRT != null)
        {
            dynamicRT.Release();
            Destroy(dynamicRT);
        }
    }
}