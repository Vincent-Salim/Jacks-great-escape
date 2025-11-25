using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          collision.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage);
        }
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          collision.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage);
        }
    }
}
