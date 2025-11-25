using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DishController : MonoBehaviour
{
    [Header("Falling")]
    [Tooltip("Fall speed in units/sec. You can change this in the inspector or from the spawner.")]
    public float fallSpeed = 2f;

    [Header("Layers / Tags")]
    public LayerMask fallingLayer;    // layer used while the dish is falling
    public LayerMask placedLayer;     // layer used once the dish is placed (static)
    public LayerMask playerLayer;     // player layer mask
    public string sinkBottomTag = "SinkBottom";

    [Header("Physics")]
    [Tooltip("Use continuous collision detection to avoid tunnelling at high speeds")]
    public CollisionDetectionMode2D collisionMode = CollisionDetectionMode2D.Continuous;

    // internal state
    [HideInInspector]
    public bool isPlaced = false;   // true when dish has been placed/stacked

    Rigidbody2D rb;
    Collider2D col;

    float spawnX;

    // small tolerance to avoid jitter
    const float UPWARD_SNAP_THRESHOLD = 0.7f; // if contact normal's Y is more than this, treat as a landing on top and snap to a static

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // initial setup: falling behaviour
        spawnX = transform.position.x;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;                 // control vertical speed
        rb.freezeRotation = true;
        rb.collisionDetectionMode = collisionMode;

        // Make sure collider is NOT a trigger so dishes collide as solid objects while falling - no more trigger
        col.isTrigger = false;

        // Put the dish onto the falling layer so it collides
        gameObject.layer = LayerMaskToLayerIndex(fallingLayer);
    }

    void FixedUpdate()
    {
        if (isPlaced) return;

        // Force a constant downward velocity (locking X to spawnX so no horizontal movement - will also do this in Unity)
        Vector2 vel = rb.linearVelocity;
        vel.x = 0f;
        vel.y = -Mathf.Abs(fallSpeed);
        rb.linearVelocity = vel;

        if (Mathf.Abs(transform.position.x - spawnX) > 0.001f)
        {
            Vector3 p = transform.position;
            p.x = spawnX;
            transform.position = p;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlaced) return;

        // If we hit the sink bottom, place the dish on it (green mug)
        if (collision.collider.CompareTag(sinkBottomTag))
        {
            PlaceOnSurface(collision.collider);
            return;
        }

        // If we hit a placed dish, decide whether to snap on top or just stop where we are. (based on that contact normal)
        if (IsInLayerMask(collision.gameObject.layer, placedLayer))
        {
            // Evaluate contact normals to see if we are landing on top
            float maxNormalY = float.NegativeInfinity;
            Vector2 highestContactPoint = Vector2.zero;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y > highestContactPoint.y)
                    highestContactPoint = contact.point;

                if (contact.normal.y > maxNormalY) maxNormalY = contact.normal.y;
            }

            // If the collision normal indicates a fairly top-down contact, we will snap neatly on top.
            if (maxNormalY >= UPWARD_SNAP_THRESHOLD)
            {
                PlaceOnSurface(collision.collider);
            }
            else
            {
                // Otherwise, just stop immediately at current position (so we have no upward snapping/jump).
                StopAndPlaceWithoutSnapping();
            }

            return;
        }
    }

    void StopAndPlaceWithoutSnapping()
    {
        // Stop movement immediately and convert to static so it won't be pushed further!!
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // Ensure it's on the placed layer for future stack detection
        gameObject.layer = LayerMaskToLayerIndex(placedLayer);

        isPlaced = true;
    }

    // Place this dish so it sits exactly on top of `surfaceCollider` (no overlap, no gap)
    void PlaceOnSurface(Collider2D surfaceCollider)
    {
        // We want to find the highest point of the surface at our spawnX.
        // Use the surface collider bounds to compute the top Y.
        float surfaceTopY = surfaceCollider.bounds.max.y;

        // Compute our half-height in world units
        float myHalfHeight = col.bounds.extents.y;

        // New Y so this dish sits exactly on top without overlapping
        Vector3 newPos = transform.position;
        newPos.y = surfaceTopY + myHalfHeight;
        newPos.x = spawnX; // keep original x

        // Raycast straight down from a little above to make sure we hit the highest placed collider at spawnX
        Vector2 castOrigin = new Vector2(spawnX, newPos.y + 0.01f);
        RaycastHit2D hit = Physics2D.Raycast(castOrigin, Vector2.down, 5f, placedLayer);
        if (hit.collider != null)
        {
            surfaceTopY = hit.collider.bounds.max.y;
            newPos.y = surfaceTopY + myHalfHeight;
        }

        // Snap into position (only downward or equal. do not move upward to avoid weird jumps)
        if (newPos.y <= transform.position.y + 0.001f)
        {
            transform.position = newPos;
        }
        else
        {
            // If snapping would move us upward, just leave at current Y (avoid the jump), but still mark placed.
        }

        // Make it an immovable static object so it cannot be pushed, or squished by future pressure
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // Ensure it's on the placed layer for future stack detection - for the next dish
        gameObject.layer = LayerMaskToLayerIndex(placedLayer);

        isPlaced = true;
    }

    // Helpers
    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    int LayerMaskToLayerIndex(LayerMask mask)
    {
        int m = mask.value;
        for (int i = 0; i < 32; i++) if ((m & (1 << i)) != 0) return i;
        return gameObject.layer; // fallback
    }

    // DEBUGGING HELP!
    void OnDrawGizmosSelected()
    {
        if (col != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(new Vector3(spawnX, transform.position.y, transform.position.z), new Vector3(col.bounds.size.x * 0.9f, 0.05f, 0f));
        }
    }
}
