using UnityEngine;
/// <summary>
/// Dish trigger area.
/// - When player is under, applies a gentle downward force to simulate dish weight.
/// - Going to use this for bubbles reduction when under dish.
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class DishTrigger : MonoBehaviour
{
    [Tooltip("Downward push applied each physics frame while overlapping player")]
    public float downwardForcePerSecond = 2f; // this is the downward force
    [Tooltip("If true, call player's OnCrushedByDish to reduce bubbles")]
    public bool callPlayerCrushMethod = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // apply small continuous downward influence
        Rigidbody2D prb = other.attachedRigidbody;
        if (prb != null)
        {
            // gently reduce vertical velocity; multiply by Time.deltaTime to be frame-rate independent
            prb.linearVelocity = new Vector2(prb.linearVelocity.x, prb.linearVelocity.y - downwardForcePerSecond * Time.deltaTime);
        }

    }
}
