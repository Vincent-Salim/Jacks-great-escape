using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAir : MonoBehaviour
{
    [Header("Air / UI (world-space sprites)")]
    [Tooltip("SpriteRenderers for the air bubbles, ordered left-to-right in the scene (rightmost pops first).")]
    [SerializeField] private SpriteRenderer[] bubbleRenderers; // left-to-right
    [Tooltip("Sprite to use when a bubble is popped.")]
    [SerializeField] private Sprite poppedBubbleSprite;
    [Tooltip("Optional: sprite used for a full bubble (if null we won't change full bubbles).")]
    [SerializeField] private Sprite fullBubbleSprite;

    [Header("Timings (editable)")]
    [Tooltip("Lose one bubble automatically every X seconds.")]
    [SerializeField] private float autoDrainInterval = 20f;
    [Tooltip("Seconds in contact before the first contact-loss occurs.")]
    [SerializeField] private float contactInitialThreshold = 2f;
    [Tooltip("Seconds between repeated losses while still in contact after the first contact loss.")]
    [SerializeField] private float contactRepeatInterval = 5f;

    [Header("Collision filter")]
    [Tooltip("Tag used to identify dish objects. Set to 'Dish' by default.")]
    [SerializeField] private string dishTag = "Dish";

    [Header("Audio")]
    [Tooltip("Optional: AudioSource to use for playing bubble sounds. If null, will try to GetComponent<AudioSource>() or fall back to PlayClipAtPoint.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Sound when a single bubble pops.")]
    [SerializeField] private AudioClip bubblePopClip;
    [Tooltip("Optional sound to play when out of air.")]
    [SerializeField] private AudioClip outOfAirClip;

    [Header("Inspector Events (hook in Inspector)")]
    [Tooltip("A simple no-argument UnityEvent (useful for legacy hooks).")]
    public UnityEvent onBubbleLostSimple;      // invoked when any bubble is lost
    public UnityEvent onOutOfAir;              // invoked when last bubble lost
    [Tooltip("UnityEvent that passes current bubbles left (int).")]
    public UnityEvent<int> onBubbleLost;       // invoked when a bubble is lost (passes bubblesLeft)
    public UnityEvent<int> onBubblesChanged;   // invoked whenever bubblesLeft updates

    public event Action<int> OnBubblesChanged;
    public event Action<int> OnBubbleLost;

    // Internal
    private int maxBubbles;
    private int bubblesLeft;
    private Coroutine autoDrainCoroutine;

    // Contact tracking
    private int contactCount = 0;   // number of dish colliders currently contacting the player (so that multiple dishes do not cause excess bubble pops)
    private bool isInContact => contactCount > 0;
    private float contactTimer = 0f; // accumulates time while in contact
    private float nextContactLossTime = Mathf.Infinity; // time threshold to next loss since contact began

    private void Awake()
    {
        if (bubbleRenderers == null || bubbleRenderers.Length == 0)
            Debug.LogWarning("PlayerAir: No bubble renderers assigned in inspector.");

        maxBubbles = bubbleRenderers != null ? bubbleRenderers.Length : 5;
        bubblesLeft = maxBubbles;


        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        StartAutoDrain();
    }

    private void OnDisable()
    {
        StopAutoDrain();
    }

    private void Start()
    {
        // ensure UI reflects current state
        RefreshBubbleUI();
    }

    private void Update()
    {
        // contact timer logic for losing bubbles while in contact with dishes
        if (isInContact && bubblesLeft > 0)
        {
            contactTimer += Time.deltaTime;

            if (contactTimer >= nextContactLossTime)
            {
                LoseOneBubble();
                nextContactLossTime += contactRepeatInterval;
            }
        }
        else
        {
            // not in contact -> reset contact
            contactTimer = 0f;
            nextContactLossTime = contactInitialThreshold; // ready for next contact
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDish(collision.gameObject))
        {
            Debug.Log("Collision (dish)");
            contactCount++;

            // if contact JUST began, reset timers so popping happens after initial threshold
            if (contactCount == 1)
            {
                contactTimer = 0f;
                nextContactLossTime = contactInitialThreshold;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsDish(collision.gameObject))
            contactCount = Mathf.Max(0, contactCount - 1);
    }

    // Originally used triggers for contact detection but switched to collisions. No need for this but keeping just in case.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDish(other.gameObject))
        {
            Debug.Log("Trigger (dish)");
            contactCount++;

            if (contactCount == 1)
            {
                contactTimer = 0f;
                nextContactLossTime = contactInitialThreshold;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsDish(other.gameObject))
            contactCount = Mathf.Max(0, contactCount - 1);
    }

    private bool IsDish(GameObject go)
    {
        if (go == null) return false;
        return go.CompareTag(dishTag);
    }

    private void StartAutoDrain()
    {
        if (autoDrainCoroutine != null) StopCoroutine(autoDrainCoroutine);
        autoDrainCoroutine = StartCoroutine(AutoDrainCoroutine());
    }

    private void StopAutoDrain()
    {
        if (autoDrainCoroutine != null)
        {
            StopCoroutine(autoDrainCoroutine);
            autoDrainCoroutine = null;
        }
    }

    private IEnumerator AutoDrainCoroutine()
    {
        while (bubblesLeft > 0)
        {
            yield return new WaitForSeconds(autoDrainInterval);
            if (bubblesLeft <= 0) yield break;
            LoseOneBubble();
        }
    }

    /// <summary>
    /// Restart the current scene immediately when run out of air.
    /// </summary>
    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    public void RestartScene(float delay)
    {
        StartCoroutine(RestartSceneCoroutine(delay));
    }

    private IEnumerator RestartSceneCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Lose one bubble and update bubbles on screen
    /// </summary>
    private void LoseOneBubble()
    {
        if (bubblesLeft <= 0) return;

        // Determine index of the rightmost full bubble.
        int bubbleIndexToPop = bubblesLeft - 1; // 0-based index for rightmost full bubble
        bubbleIndexToPop = Mathf.Clamp(bubbleIndexToPop, 0, bubbleRenderers.Length - 1);

        SpriteRenderer sr = bubbleRenderers[bubbleIndexToPop];
        if (sr != null)
        {
            if (poppedBubbleSprite != null)
                sr.sprite = poppedBubbleSprite;
            else
                sr.enabled = false;
        }

        // Play pop sound
        PlayBubblePopSound();

        bubblesLeft = Mathf.Max(0, bubblesLeft - 1);

        // Events - inspector hooks
        onBubbleLostSimple?.Invoke();
        onBubbleLost?.Invoke(bubblesLeft);
        onBubblesChanged?.Invoke(bubblesLeft);

        // Events - code subscribers
        OnBubbleLost?.Invoke(bubblesLeft);
        OnBubblesChanged?.Invoke(bubblesLeft);

        if (bubblesLeft <= 0)
        {
            PlayOutOfAirSound();

            onOutOfAir?.Invoke();
            StopAutoDrain();
            Debug.Log("Out of air!");
        }

        if (bubblesLeft <= 0)
        {
            StopAutoDrain();

            // Play sound immediately
            PlayOutOfAirSound();

            // Start a short delay before triggering restart or out-of-air event
            StartCoroutine(HandleOutOfAirDelay(1.5f));
        }
    }

    // Rebuild UI from the bubblesLeft value.
    private void RefreshBubbleUI()
    {
        int fullCount = Mathf.Clamp(bubblesLeft, 0, bubbleRenderers.Length);

        for (int i = 0; i < bubbleRenderers.Length; i++)
        {
            SpriteRenderer sr = bubbleRenderers[i];
            if (sr == null) continue;

            // left-to-right: indexes < fullCount are full, others are popped!
            if (i < fullCount)
            {
                if (fullBubbleSprite != null)
                    sr.sprite = fullBubbleSprite;
                sr.enabled = true;
            }
            else
            {
                if (poppedBubbleSprite != null)
                    sr.sprite = poppedBubbleSprite;
                else
                    sr.enabled = false;
            }
        }

        // Notify listeners of current state
        onBubblesChanged?.Invoke(bubblesLeft);
        OnBubblesChanged?.Invoke(bubblesLeft);
    }

    #region Audio helpers
    private void PlayBubblePopSound()
    {
        if (bubblePopClip == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(bubblePopClip);
        }
        else
        {
            // fallback: play at camera position
            Vector3 pos = (Camera.main != null) ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(bubblePopClip, pos);
        }
    }

    private void PlayOutOfAirSound()
    {
        if (outOfAirClip == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(outOfAirClip);
        }
        else
        {
            Vector3 pos = (Camera.main != null) ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(outOfAirClip, pos);
        }
    }

    private IEnumerator HandleOutOfAirDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Now invoke the event and restart!!
            onOutOfAir?.Invoke();

        }
    #endregion

    #region Public API
    /// <summary>
    /// Refill all bubbles to full.
    /// </summary>
    public void RefillAllBubbles()
    {
        bubblesLeft = maxBubbles;
        RefreshBubbleUI();
        StartAutoDrain();
    }

    /// <summary>
    /// Give X bubbles.
    /// </summary>
    public void AddBubbles(int amount)
    {
        bubblesLeft = Mathf.Clamp(bubblesLeft + amount, 0, maxBubbles);
        RefreshBubbleUI();
        if (bubblesLeft > 0 && autoDrainCoroutine == null)
            StartAutoDrain();
    }

    /// <summary>
    /// Get how many bubbles remain.
    /// </summary>
    public int GetBubblesLeft() => bubblesLeft;
    #endregion
}
