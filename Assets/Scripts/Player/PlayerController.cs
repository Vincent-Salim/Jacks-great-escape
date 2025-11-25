using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerRespawn PlayerRespawn;

    [Header("Player Settings")]
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpingPower = 5f;

    // Expose the configured max move speed so other systems (like predictive traps) can use it.
    public float MaxMoveSpeed => speed;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundCheckSize = new Vector2(1f, 0.1f);

    [Header("Invulnerability")]
    [SerializeField] float invincibleDuration = 0.8f;
    [SerializeField] static int maxHealth = 3;

    private int health;
    private bool invincible = false;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer spriteRenderer;
    Color originalColor;
    public static PlayerController Instance;

    [Header("Animation / Sprites")]
    [SerializeField] Animator animator; // optional - if assigned, animator gets parameters set
    // Animator parameter names (ensure these exist in your Animator Controller if you're using it)
    const string ANIM_PARAM_SPEED = "Speed";
    const string ANIM_PARAM_IS_GROUNDED = "IsGrounded";
    const string ANIM_PARAM_IS_SWIMMING = "IsSwimming";

    // Sprite-sheet fallback (used if no animator or if you prefer sprite animation)
    [Header("Sprite Animation (fallback)")]
    [Tooltip("Ordered frames for running animation (assign sliced frames or separate sprites).")]
    public Sprite[] runSprites;
    [Tooltip("Ordered frames for swimming animation (assign sliced frames or separate sprites).")]
    public Sprite[] swimSprites;
    [Tooltip("Ordered frames for idle animation (assign sliced frames or separate sprites).")]
    public Sprite[] idleSprites;
    [Header("Falling")]
    [Tooltip("When Jack enters the sink, this downward velocity will be applied (if >0).")]
    public float fallVelocity = -8f; // negative = down
    [Tooltip("If true, player control is disabled while falling.")]
    public bool disableControlWhileFalling = true;
    [Tooltip("Optional single sprite for falling if not using Animator")]
    public Sprite fallSprite;

    [Tooltip("Frames per second for running animation when using sprite fallback.")]
    public float runFPS = 12f;
    [Tooltip("Frames per second for swimming animation when using sprite fallback.")]
    public float swimFPS = 12f;
    [Tooltip("Frames per second for idle animation when using sprite fallback.")]
    public float idleFPS = 6f;

    // movement threshold to count as "idle"
    [Tooltip("Horizontal speed below which Jack is considered idle (for sprite fallback).")]
    public float idleMovementThreshold = 0.1f;

    // swimming physics tweaks
    [Header("Swimming Physics")]
    [Tooltip("If > 0, this overrides Rigidbody2D.gravityScale while swimming.")]
    public float swimGravityScale = 0.5f;
    [Tooltip("If > 0, this sets Rigidbody2D.drag while swimming.")]
    public float swimDrag = 2f;
    float normalGravityScale;
    float normalDrag;

    // internal animation state
    Coroutine animCoroutine;
    Sprite[] currentSequence;
    float currentFPS;
    bool isSwimming = false;
    bool isFalling = false;
    bool controlsEnabled = true;

    [Header("Health")]
    [SerializeField] Image[] drawnHearts = new Image[maxHealth];

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField][Range(0f, 1f)] private float damageSoundVolume = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        health = maxHealth;

        if (PlayerRespawn == null)
            PlayerRespawn = GetComponent<PlayerRespawn>();

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                Debug.LogError("[PlayerController] Rigidbody2D is not assigned.");
        }

        // save normal physics values
        if (rb != null)
        {
            normalGravityScale = rb.gravityScale;
            normalDrag = rb.linearDamping;
        }

        // start in running/idle state by default
        StartRunning();
    }

    private Vector2 moveInput;

    private void FixedUpdate()
    {
        // apply horizontal movement using Rigidbody2D.velocity
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        }

        // update animator parameters (use absolute horizontal speed for run/idle)
        float currentHorizontalSpeed = rb != null ? Mathf.Abs(rb.linearVelocity.x) : Mathf.Abs(moveInput.x * speed);

        if (animator != null)
        {
            animator.SetFloat(ANIM_PARAM_SPEED, currentHorizontalSpeed);
            animator.SetBool(ANIM_PARAM_IS_GROUNDED, IsGrounded());
            animator.SetBool(ANIM_PARAM_IS_SWIMMING, isSwimming);
        }
        else
        {
            // Sprite-fallback automatic sequence selection:
            if (isSwimming)
            {
                EnsureSpriteSequence(swimSprites, swimFPS);
            }
            else
            {
                bool grounded = IsGrounded();
                if (grounded && currentHorizontalSpeed <= idleMovementThreshold)
                {
                    EnsureSpriteSequence(idleSprites, idleFPS);
                }
                else
                {
                    EnsureSpriteSequence(runSprites, runFPS);
                }
            }
        }

        // flip sprite based on input (call in FixedUpdate so it's in sync with movement)
        if (spriteRenderer != null)
        {
            if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;
        }
    }

    // Input System callbacks
    public void OnMove(InputValue value)
    {
        if (!controlsEnabled)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!controlsEnabled) return;

        if (value.isPressed && IsGrounded())
        {
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer) != null;
    }

    private void PlayDamageSound()
    {
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound, damageSoundVolume);
        }
    }

    #region Damage / Health
    public void TakeDamage(int amount = 1)
    {
        if (invincible) return;

        Debug.Log("Player is taking damage");

        invincible = true;
        health -= amount;
        Debug.Log($"Player took {amount} damage, health now {health}");
        if (PlayerRespawn == null)
        {
            Debug.LogError("PlayerRespawn is NULL! Cannot respawn.");
            return;
        }
        // flash red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        PlayDamageSound();

        if (health <= 0)
        {
            Debug.Log("Player has died!");
            StartCoroutine(FadeToBlackCoroutine());
        }
        else
        {
            if (health >= 0 && health < drawnHearts.Length)
                drawnHearts[health].enabled = false;
        }

        StartCoroutine(InvincibilityCoroutine());
    }

    public void ResetHealth()
    {
        health = maxHealth;
        for (int i = 0; i < drawnHearts.Length; i++)
        {
            drawnHearts[i].enabled = true;
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        invincible = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    #endregion

    #region Fade / Respawn
    private IEnumerator FadeToBlackCoroutine()
    {
        moveInput = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        enabled = false;
        if (animator != null) animator.SetFloat(ANIM_PARAM_SPEED, 0f);

        yield return ScreenFader.Instance.FadeOut();

        enabled = true;
        PlayerRespawn.Respawn();

        // Fade back in
        yield return ScreenFader.Instance.FadeIn();
    }
    #endregion

    #region Animation control

    public void StartRunning()
    {
        // If already in running state and sequence matches, do nothing
        if (!isSwimming && currentSequence == runSprites && animCoroutine != null) return;

        isSwimming = false;
        ApplySwimmingPhysics(false);

        // Animator will handle visuals
        if (animator != null)
        {
            animator.SetBool(ANIM_PARAM_IS_SWIMMING, false);
        }
        else
        {
            // Just in case! sprite fallback: choose run or idle immediately based on current speed.
            float currentHorizontalSpeed = rb != null ? Mathf.Abs(rb.linearVelocity.x) : Mathf.Abs(moveInput.x * speed);
            if (IsGrounded() && currentHorizontalSpeed <= idleMovementThreshold)
                SwitchSpriteSequence(idleSprites, idleFPS);
            else
                SwitchSpriteSequence(runSprites, runFPS);
        }
    }

    public void StartSwimming()
    {
        // Should only be called in level 2 when in water
        if (isSwimming && currentSequence == swimSprites && animCoroutine != null) return;

        isSwimming = true;
        ApplySwimmingPhysics(true);

        if (animator != null)
        {
            animator.SetBool(ANIM_PARAM_IS_SWIMMING, true);
        }
        else
        {
            // Sprite fallback: switch to swim sequence
            SwitchSpriteSequence(swimSprites, swimFPS);
        }
    }

    public void StartFalling()
    {
        if (isFalling) return;
        isFalling = true;

        // freeze horizontal control
        if (disableControlWhileFalling) controlsEnabled = false;

        // Set param so Animator plays falling clip
        if (animator != null)
        {
            animator.SetBool("IsFalling", true);
            animator.enabled = true;
        }
        else
        {
            // Just in case sprite-fallback: stop other sprite anim and show the fall sprite
            StopSpriteAnimation();
            if (fallSprite != null && spriteRenderer != null) spriteRenderer.sprite = fallSprite;
        }

        // Apply immediate downward velocity.
        if (rb != null && fallVelocity != 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fallVelocity);
        }
    }

    public void OnHitSinkBottom()
    {
        // Called when Jack collides with the bottom
        isFalling = false;
        controlsEnabled = true; // re-enable movement

        // animator-  clear falling, start swimming
        if (animator != null)
        {
            animator.SetBool("IsFalling", false);
            StartSwimming(); // this sets IsSwimming param
        }
        else
        {
            // sprite fallback: switch to swim sprites
            StartSwimming();
        }
    }


    /// <summary>
    /// Force set a single sprite (stop animation).
    /// </summary>
    public void SetStaticSprite(Sprite sprite)
    {
        StopSpriteAnimation();
        if (sprite != null && spriteRenderer != null) spriteRenderer.sprite = sprite;
    }

    void ApplySwimmingPhysics(bool swimming)
    {
        // adjust Rigidbody2D physics for swimming vs normal
        if (rb == null) return;
        if (swimming)
        {
            if (swimGravityScale > 0f) rb.gravityScale = swimGravityScale;
            if (swimDrag > 0f) rb.linearDamping = swimDrag;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
            rb.linearDamping = normalDrag;
        }
    }

    #region Sprite-fallback animation
    void EnsureSpriteSequence(Sprite[] sequence, float fps)
    {
        if (sequence == null || sequence.Length == 0)
        {
            // nothing to ensure
            return;
        }

        // if already the sequence, do nothing
        if (currentSequence == sequence && animCoroutine != null) return;

        SwitchSpriteSequence(sequence, fps);
    }

    void SwitchSpriteSequence(Sprite[] sequence, float fps)
    {
        if (sequence == null || sequence.Length == 0)
        {
            // nothing to animate, stop current
            StopSpriteAnimation();
            return;
        }

        currentSequence = sequence;
        currentFPS = fps > 0f ? fps : 12f;

        StopSpriteAnimation();
        animCoroutine = StartCoroutine(AnimateSequence(currentSequence, currentFPS));
    }

    void StopSpriteAnimation()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }

    IEnumerator AnimateSequence(Sprite[] seq, float fps)
    {
        int idx = 0;
        float wait = 1f / fps;
        while (true)
        {
            if (seq != null && seq.Length > 0 && spriteRenderer != null)
            {
                spriteRenderer.sprite = seq[idx];
                idx = (idx + 1) % seq.Length;
            }
            yield return new WaitForSeconds(wait);
        }
    }
    #endregion

    #endregion

    #region Trigger handlers (water)
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("SinkEntry"))
        {
            // Start falling when the player enters the sink opening
            StartFalling();
            return;
        }

        if (other.CompareTag("SinkBottom"))
        {
            // when hitting bottom, go to swimming!!
            OnHitSinkBottom();
            return;
        }
    }

    #endregion
}
