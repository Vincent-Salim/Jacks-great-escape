using UnityEngine;

public class FinishPointArrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject arrowSprite;
    
    [Header("Arrow Settings")]
    [SerializeField] private float distanceFromPlayer = 2f;
    [SerializeField] private float minDistanceToShow = 5f;
    [SerializeField] private bool rotateArrowSprite = true;
    [SerializeField] private float rotationOffset = 0f;
    
    private Transform finishPoint;
    private SpriteRenderer arrowRenderer;
    
    void Start()
    {
        // Find the finish point in the scene
        FinishPoint finish = FindObjectOfType<FinishPoint>();
        if (finish != null)
        {
            finishPoint = finish.transform;
        }
        else
        {
            Debug.LogError("No FinishPoint found in scene!");
            enabled = false;
            return;
        }
        
        // Get player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Get arrow sprite renderer
        if (arrowSprite != null)
        {
            arrowRenderer = arrowSprite.GetComponent<SpriteRenderer>();
        }
    }
    
    void Update()
    {
        if (player == null || finishPoint == null || arrowSprite == null) return;
        
        // Calculate distance to finish
        float distanceToFinish = Vector2.Distance(player.position, finishPoint.position);
        // Debug.Log($"Distance to finish: {distanceToFinish}");

        // Hide arrow if too close to finish
        if (distanceToFinish < minDistanceToShow)
        {
            arrowSprite.SetActive(false);
            return;
        }
        else
        {
            arrowSprite.SetActive(true);
        }
        
        // Debug.Log($"Player position: {player.position}");

        // Calculate direction from player to finish
        Vector2 directionToFinish = (finishPoint.position - player.position).normalized;
        
        Vector3 arrowPosition = player.position + (Vector3)(directionToFinish * distanceFromPlayer);
        arrowPosition.z = player.position.z + 1;
        arrowSprite.transform.position = arrowPosition; 
        // Debug.Log($"Arrow should be at: {arrowPosition}");

        // Rotate arrow to point at finish
        if (rotateArrowSprite)
        {
            float angle = Mathf.Atan2(directionToFinish.y, directionToFinish.x) * Mathf.Rad2Deg;
            arrowSprite.transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
        
        if (arrowRenderer != null)
        {
            float fadeDistance = 20f;
            float alpha = Mathf.Clamp01(distanceToFinish / fadeDistance);
            Color color = arrowRenderer.color;
            color.a = alpha;
            arrowRenderer.color = color;
        }
    }
}