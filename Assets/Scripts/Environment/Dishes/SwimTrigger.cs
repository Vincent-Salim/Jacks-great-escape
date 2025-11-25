// NO LONGER USING - DELETE - SEE PLAYERCONTROLLER.CS FOR UPDATED VERSION
using UnityEngine;


// I want this all in one script with the running + jumping - need to reorganise later


/// <summary>
/// Attach to the bottom/top trigger objects in the sink. When Player enters:
/// - Bottom trigger: switch to swimming mode (enable PlayerSwimming, disable PlayerController)
/// - Top trigger: switch back to walking mode
/// Includes simple guards to avoid accidental toggles.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WaterTrigger : MonoBehaviour
{
    public enum TriggerRole { BottomStartSwim, TopEndSwim }
    public TriggerRole role = TriggerRole.BottomStartSwim;

    [Tooltip("Player tag (make sure your player has this tag).")]
    public string playerTag = "Player";

    [Tooltip("Optional: require player vertical direction for activation (true = require entering from above for BottomStartSwim).")]
    public bool requireDirectionForBottom = true;

    [Tooltip("Optional cooldown (seconds) after toggling to ignore rapid toggles.")]
    public float toggleCooldown = 0.25f;

    float lastToggleTime = -10f;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (Time.time - lastToggleTime < toggleCooldown) return;

        var player = other.gameObject;
        var controller = player.GetComponent<PlayerController>();
        var swimmer = player.GetComponent<PlayerSwimming>();

        // If PlayerSwimming component is missing but Bottom trigger tries to start swim, optionally add it
        if (swimmer == null && role == TriggerRole.BottomStartSwim)
        {
            swimmer = player.AddComponent<PlayerSwimming>();
            swimmer.enabled = false; // we'll enable below
        }

        // Check direction if needed (only relevant for bottom trigger)
        if (role == TriggerRole.BottomStartSwim && requireDirectionForBottom)
        {
            // Only trigger if player is moving downward (fell into sink)
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Use linearVelocity (Unity 6+)
                if (rb.linearVelocity.y > 0.2f)
                {
                    // moving upward - ignore
                    return;
                }
            }
        }

        if (role == TriggerRole.BottomStartSwim)
        {
            // Start swimming
            if (swimmer != null) swimmer.enabled = true;
            if (controller != null) controller.enabled = false;

            // zero vertical velocity so swim starts cleanly
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            lastToggleTime = Time.time;
        }
        else if (role == TriggerRole.TopEndSwim)
        {
            // End swimming (player reached top)
            if (swimmer != null) swimmer.enabled = false;
            if (controller != null) controller.enabled = true;

            // zero vertical velocity to avoid flying out
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            lastToggleTime = Time.time;
        }
    }
}

