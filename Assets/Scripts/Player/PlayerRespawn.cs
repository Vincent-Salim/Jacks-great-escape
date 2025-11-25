using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
        Debug.Log("Trying to respawn");
        if (currentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint set! Cannot respawn.");
            return;
        }
        transform.position = currentCheckpoint.position;
        playerController.ResetHealth();
    }

    // Activate checkpoint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"HOLA Collided with: {collision.transform.tag}");
        if (collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform;
            Debug.Log($"Checkpoint set: {currentCheckpoint}");
            // SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
        }
    }
}
