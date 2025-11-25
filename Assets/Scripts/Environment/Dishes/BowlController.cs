// NO LONGER USING - DELETE - SEE DISHCONTROLLER.CS FOR UPDATED VERSION
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BowlController : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Set the bowl's sprite (colour). Safe to call at spawn time.
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"[{name}] SetSprite called with null sprite.");
            return;
        }
        sr.sprite = sprite;
        // make sure renderer is visible
        sr.enabled = true;
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

}

