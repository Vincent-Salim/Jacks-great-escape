using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 spriteSize;
    public GameObject cam;
    public Vector2 parallaxFactor; // X controls horizontal movement, Y controls vertical

    void Start()
    {
        startPos = transform.position;

        // Get the sprite size (width and height) for infinite looping
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteSize = sr.bounds.size;
    }

    void Update()
    {
        // Calculate temporary positions for X and Y
        float tempX = cam.transform.position.x * (1 - parallaxFactor.x);
        float distX = cam.transform.position.x * parallaxFactor.x;

        float tempY = cam.transform.position.y * (1 - parallaxFactor.y);
        float distY = cam.transform.position.y * parallaxFactor.y;

        // Apply the parallax movement on both axes
        transform.position = new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z);

        // Infinite horizontal looping
        if (Mathf.Abs(cam.transform.position.x - startPos.x) >= spriteSize.x)
        {
            float offsetX = (cam.transform.position.x - startPos.x) % spriteSize.x;
            startPos.x = cam.transform.position.x + offsetX;
        }

        // Infinite vertical looping (optional, only if you have tall repeating backgrounds)
        if (Mathf.Abs(cam.transform.position.y - startPos.y) >= spriteSize.y)
        {
            float offsetY = (cam.transform.position.y - startPos.y) % spriteSize.y;
            startPos.y = cam.transform.position.y + offsetY;
        }
    }
}
