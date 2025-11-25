using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple swimming controller for player.
/// - Auto-rises at a constant speed.
/// - Player can move left/right to avoid dishes.
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwimming : MonoBehaviour
{
    public Rigidbody2D rb;
    public float autoRiseSpeed = 1.8f;
    public float horizontalSpeed = 3.5f;
    public float manualDownSpeed = 2.5f;

    public SpriteRenderer spriteRenderer;
    private Vector2 moveInput = Vector2.zero;

    void Reset() { rb = GetComponent<Rigidbody2D>(); }

    void FixedUpdate()
    {
        float vx = moveInput.x * horizontalSpeed;
        bool pressingDown = moveInput.y < -0.2f;

        float vy = pressingDown ? -manualDownSpeed : autoRiseSpeed;

        if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
        else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;
        // Apply the swimming velocity
        rb.linearVelocity = new Vector2(vx, vy);
    }

    // Player can move left and right
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Provide an API for DishTrigger to call
    public void SetUnderDish(bool under)
    {
        if (under)
        {
            // immediate forced downwards
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -4f);
        }
    }
}

