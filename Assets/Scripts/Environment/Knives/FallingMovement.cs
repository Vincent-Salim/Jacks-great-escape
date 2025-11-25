using System.Collections;
using UnityEngine;
public class FallingMovement : EnemyDamage
{
    private Transform target;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    [SerializeField] float distance = 6f;
    [SerializeField] float acceleration = 9.8f;
    [SerializeField] float accelerationMultiplier = 6f;
    bool isFalling = false;
    bool isResetting = false;
    Vector3 startPosition;
    float startRotation;
    float originalGravityScale = 0f;
    RigidbodyType2D originalBodyType = RigidbodyType2D.Dynamic;
    [SerializeField] float resetDelay = 1f; // seconds before the knife resets to its start position
    [SerializeField] float resetMoveDuration = 0.4f; // time it takes to move back up to start

    [Header("Audio")]
    [SerializeField] private AudioClip fallingSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private enum MovementType
    {
        Constant,
        Predictive,
        Reactive
    }
    [SerializeField] MovementType movementType = MovementType.Constant;

    Vector2 currentVelocity = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        boxCollider = GetComponent<BoxCollider2D>();
        startPosition = transform.position;
        startRotation = transform.eulerAngles.z;
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
            originalBodyType = rb.bodyType;
            rb.gravityScale = 0f; 
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    private bool IsVisibleToCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) 
        {
            return false;
        }
        
        Vector3 viewportPoint = cam.WorldToViewportPoint(transform.position);
        
        // Check if within viewport with small buffer
        bool inView = viewportPoint.x >= -0.1f && viewportPoint.x <= 1.1f && 
                    viewportPoint.y >= -0.1f && viewportPoint.y <= 1.1f && 
                    viewportPoint.z > 0;
        
        return inView;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        Physics2D.queriesStartInColliders = false;
        if (isFalling)
        {
            Vector2 direction = transform.up;
            currentVelocity += direction * accelerationMultiplier * acceleration * Time.fixedDeltaTime;
            rb.linearVelocity = currentVelocity;
            return;
        }
        if (isFalling == false && isResetting == false)
        {
            RaycastHit2D hit;

            if (movementType == MovementType.Constant)
            {
                StartFalling();
            }
            else
            {
                if (movementType == MovementType.Reactive)
                {
                    hit = Physics2D.Raycast(transform.position, Vector2.down, distance);
                    Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);
                }
                else // predictive: will the knife hit the player's future position if the player keeps moving?
                {
                    var playerRb = target != null ? target.GetComponent<Rigidbody2D>() : null;
                    var playerCollider = target != null ? target.GetComponent<Collider2D>() : null;

                    // Estimate fall time from knife to player's current vertical position using gravity that will be applied (acceleration as gravityScale)
                    float fallTime = 0f;
                    float deltaY = transform.position.y - target.position.y;
                    if (deltaY > 0f && acceleration > 0f)
                    {
                        fallTime = Mathf.Sqrt(2f * deltaY / (accelerationMultiplier * acceleration));
                    }

                    // Get player's current horizontal velocity and clamp to player's configured max acceleration if available
                    float playerVelX = playerRb != null ? playerRb.linearVelocity.x : 0f;
                    float playerMaxacceleration = PlayerController.Instance != null ? PlayerController.Instance.MaxMoveSpeed : Mathf.Abs(playerVelX);
                    playerVelX = Mathf.Clamp(playerVelX, -playerMaxacceleration, playerMaxacceleration);

                    // Compute sweep range for the player over fallTime
                    float tolerance = 0.8f;
                    float playerStartX = target.position.x;
                    float playerEndX = playerStartX + tolerance * playerVelX * fallTime;
                    float playerMinX = Mathf.Min(playerStartX, playerEndX);
                    float playerMaxX = Mathf.Max(playerStartX, playerEndX);

                    // Determine player's half-width
                    float playerHalfWidth = 0.5f;
                    if (playerCollider != null)
                    {
                        playerHalfWidth = playerCollider.bounds.extents.x;
                    }

                    float knifeX = transform.position.x;

                    bool wouldOverlapSweep = (knifeX) >= (playerMinX) && (knifeX) <= (playerMaxX);
                    bool withinVerticalRange = deltaY >= 0f && deltaY <= distance;

                    // If the sweep test indicates the player's path will cross the knife X, do a short raycast just above the
                    // predicted player position so the resulting RaycastHit2D references the player's collider directly.
                    if (wouldOverlapSweep && withinVerticalRange)
                    {
                        // Use the player's predicted center X as the origin for a very short downward ray so it hits the player
                        Vector2 rayOrigin = new Vector2(target.position.x, target.position.y + 3f);
                        float rayLen = 3f;
                        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLen);
                        Debug.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLen, Color.magenta);
                    }
                    else
                    {
                        hit = new RaycastHit2D();
                    }

                    Debug.DrawLine(new Vector3(playerStartX, target.position.y, 0f), new Vector3(playerEndX, target.position.y, 0f), Color.yellow);
                    Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 0f), new Vector3(transform.position.x, transform.position.y - distance, 0f), Color.cyan);
                }
                // go down if we hit the player
                if (hit.transform != null)
                {
                    if (hit.transform.tag == "Player")
                    {
                        StartFalling();
                    }
                }
            }
        }
    }
    private void StartFalling()
    {
        isFalling = true;
        currentVelocity = Vector2.zero;
        
        if (fallingSound != null && audioSource != null && IsVisibleToCamera())
        {
            audioSource.volume = soundVolume;  // Set volume directly
            audioSource.clip = fallingSound;
            audioSource.Play();
        }
    }

    // If the knife's collider is set to 'Is Trigger' this will catch overlaps with the ground
    // and behave similarly to OnCollisionEnter2D. Note: trigger overlaps won't produce a physical
    // collision response, so the knife may briefly overlap the ground before ResetAfterCollision moves it.
    private new void OnCollisionEnter2D(Collision2D other)
    {
        if (!isFalling || isResetting) return;

        base.OnCollisionEnter2D(other);

        StartCoroutine(ResetAfterCollision());
    }

    IEnumerator ResetAfterCollision()
    {
        isResetting = true;
        isFalling = false;

        // Stop movement immediately

        // Wait a short time so any collision responses (sound/damage) can happen
        yield return new WaitForSeconds(resetDelay);

        // Smoothly move the knife back up to its start position instead of teleporting.
        if (rb != null)
        {
            // Make kinematic so physics won't interfere with MovePosition
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Vector2 from = rb.position;
            Vector2 to = startPosition;
            float elapsed = 0f;
            float duration = Mathf.Max(0.001f, resetMoveDuration);

            while (elapsed < duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                Vector2 newPos = Vector2.Lerp(from, to, t);
                rb.MovePosition(newPos);
                yield return new WaitForFixedUpdate();
            }

            // Ensure final position
            rb.MovePosition(to);

            // Restore physics
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = originalGravityScale;
            rb.bodyType = originalBodyType;
        }
        else
        {
            // Fallback: directly set transform if no Rigidbody present
            float elapsed = 0f;
            float duration = Mathf.Max(0.001f, resetMoveDuration);
            Vector3 from = transform.position;
            Vector3 to = startPosition;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }
            transform.position = startPosition;
        }

        // Ensure rotation snapped back
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startRotation);

        isResetting = false;
    }
}
