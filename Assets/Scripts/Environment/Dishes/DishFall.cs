// NO LONGER USING - DELETE - SEE DISHCONTROLLER.CS FOR UPDATED VERSION
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DishFall : MonoBehaviour
{
    [Header("Motion")]
    public float fallSpeed = 5f; 
    public bool useGravity = false;              

    [Header("Landing / snapping")]
    [Tooltip("Layers that can support a dish (e.g. Sink floor, already-landed dishes).")]
    public LayerMask supportLayers;
    [Tooltip("Minimum contact normal.y to be considered a support contact.")]
    [Range(0f, 1f)] public float minSupportNormalY = 0.5f;
    [Tooltip("How many seconds of continuous support before we freeze the dish.")]
    public float requiredSupportTime = 0.08f;
    [Tooltip("Small offset applied when snapping to avoid slight overlap.")]
    public float snapTolerance = 0.02f;

    [Header("Optional")]
    [Tooltip("If a collider is on any of these layers, it will NOT be counted as support (e.g. Player).")]
    public LayerMask ignoreAsSupportLayers;

    // runtime
    private Rigidbody2D rb;
    private Collider2D col;

    // track supporting colliders and support time
    private readonly HashSet<Collider2D> supportingColliders = new HashSet<Collider2D>();
    private float currentSupportTimer = 0f;
    private bool landed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Ensure no bounciness by default
        if (rb.sharedMaterial == null)
            rb.sharedMaterial = new PhysicsMaterial2D { bounciness = 0f, friction = 0f };
    }

    private void Start()
    {
        if (!useGravity)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.down * fallSpeed;
        }
        else
        {
            // allow gravity but clamp initial downward velocity
            rb.gravityScale = 1f;
        }
    }

    private void FixedUpdate()
    {
        if (landed) return;

        if (!useGravity)
        {
            // only set initial downward linearVelocity if it's still near that speed
            // we don't overwrite each frame so physics solver can react to collisions
            if (rb.linearVelocity.y > -fallSpeed)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fallSpeed);
        }

        if (supportingColliders.Count > 0)
        {
            currentSupportTimer += Time.fixedDeltaTime;
            if (currentSupportTimer >= requiredSupportTime)
                DoLand();
        }
        else
        {
            currentSupportTimer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollisionForSupport(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollisionForSupport(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (supportingColliders.Contains(collision.collider))
            supportingColliders.Remove(collision.collider);
    }

    private void EvaluateCollisionForSupport(Collision2D collision)
    {
        if (landed) return;

        if (((1 << collision.collider.gameObject.layer) & ignoreAsSupportLayers) != 0)
            return;

        // check average contact normal to ensure collision is beneath the dish (normal pointing up)
        Vector2 avgNormal = Vector2.zero;
        foreach (var contact in collision.contacts)
            avgNormal += contact.normal;
        avgNormal /= collision.contactCount;

        if (avgNormal.y < minSupportNormalY)
            return; // not a supporting contact (e.g. side collision)

        // now test whether the other object is allowed to support:
        GameObject other = collision.collider.gameObject;

        bool otherIsSupportLayer = ((1 << other.layer) & supportLayers) != 0;

        // If other is a dish with DishFall and has already landed, it's a valid support.
        var otherDish = other.GetComponent<DishFall>();
        bool otherDishLanded = otherDish != null && otherDish.landed;

        if (otherIsSupportLayer || otherDishLanded)
        {
            // we add to supporting colliders set (so we require sustained contact)
            supportingColliders.Add(collision.collider);
            // support timer will be accumulated in FixedUpdate
        }
    }

    private void DoLand()
    {
        // final safety: make sure we're not colliding with ignored layers (player) as sole support
        if (supportingColliders.Count == 0) return;

        // Snap slightly along Y to reduce micro-overlap - not working
        transform.position = new Vector3(transform.position.x, transform.position.y + snapTolerance, transform.position.z);

        // stop movement and switch to Static so it stops contributing solver pressure - NOT WORKING WELL - FIX
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;

        landed = true;

    }

    public bool IsLanded() => landed;
}
