using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(Animator))]
public class FireTrap : MonoBehaviour
{
    [Header("Fire Settings")]
    public float activeTime = 1.5f;    // fire visible and harmful
    public float inactiveTime = 2f;    // fire hidden and safe
    
    [Header("Smoke Settings")]
    [SerializeField] private ParticleSystem smokeParticles; 
    [SerializeField] private float smokeStartDelay = 0.5f; 
    [SerializeField] private float smokeLingerTime = 0.3f; 

    [Header("Audio")]
    [SerializeField] private AudioClip fireStartSound;
    [SerializeField] private AudioClip fireLoopSound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 0.7f;
    private AudioSource audioSource;

    private bool isActive = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        if (smokeParticles != null)
        {
            smokeParticles.Stop();
        }

        StartCoroutine(FireCycle());
    }

    private bool IsVisibleToCamera()
    {
        // Method 1: Use sprite renderer visibility
        if (spriteRenderer != null && spriteRenderer.isVisible)
            return true;
            
        // Method 2: Manual viewport check as fallback
        Camera cam = Camera.main;
        if (cam == null) return false;
        
        Vector3 viewportPoint = cam.WorldToViewportPoint(transform.position);
        
        // Check if within viewport with small buffer
        return viewportPoint.x >= -0.1f && viewportPoint.x <= 1.1f && 
               viewportPoint.y >= -0.1f && viewportPoint.y <= 1.1f && 
               viewportPoint.z > 0;
    }

    private IEnumerator FireCycle()
    {
        while (true)
        {
            if (smokeParticles != null)
            {
                smokeParticles.Play();
            }
            
            yield return new WaitForSeconds(smokeStartDelay);
            
            isActive = true;
            spriteRenderer.enabled = true;
            animator.enabled = true;
            PlayFireSound();

            yield return new WaitForSeconds(activeTime);

            isActive = false;
            spriteRenderer.enabled = false;
            animator.enabled = false;
            StopFireSound();
            
            yield return new WaitForSeconds(smokeLingerTime);
            
            if (smokeParticles != null)
            {
                smokeParticles.Stop();
            }
            
            yield return new WaitForSeconds(inactiveTime - smokeLingerTime);
        }
    }

    private void PlayFireSound()
    {
        if (audioSource == null) return;

        // Only play sounds if visible on screen
        if (!IsVisibleToCamera()) return;

        if (fireStartSound != null)
        {
            audioSource.PlayOneShot(fireStartSound, soundVolume);
        }

        if (fireLoopSound != null)
        {
            audioSource.clip = fireLoopSound;
            audioSource.loop = true;
            audioSource.volume = soundVolume * 0.5f;
            audioSource.Play();
        }
    }

    private void StopFireSound()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;

        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(1);
        }
    }
}