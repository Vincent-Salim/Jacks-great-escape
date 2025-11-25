using UnityEngine;
using System.Collections;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private float delayBeforeNextLevel = 0.5f; // Adjustable delay
    
    private AudioSource audioSource;
    private bool hasTriggered = false; // Prevent multiple triggers
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // If no AudioSource exists, add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Prevent triggering multiple times
            StartCoroutine(FinishSequence());
        }
    }
    
    private IEnumerator FinishSequence()
    {
        // Play the sound
        if (finishSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(finishSound);
            yield return new WaitForSeconds(delayBeforeNextLevel);
            yield return ScreenFader.Instance.FadeOut(1f);
        }
        
        // Go to next level
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneController.instance.LoadScene(nextLevelName);
        }
        else
        {
            SceneController.instance.NextLevel();
        }
    }
}