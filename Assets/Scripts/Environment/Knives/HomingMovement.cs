using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMovement : EnemyDamage
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 5f;
    [SerializeField] float rotateSpeed = 2000f;
    [SerializeField] Vector2 offset = Vector2.zero;
    [SerializeField] HomingType homingType = HomingType.Constant;
    [Header("Slash Settings")]
    [SerializeField] float lockOnDuration = 1f;
    [SerializeField] float slashSpeedMultiplier = 3f;
    [SerializeField] float slashDuration = 0.2f;
    [SerializeField] float pauseDuration = 0.2f;

    enum HomingType { Constant, Slash };
    private Rigidbody2D rb;
    private bool isSlashing = false;
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        rb = GetComponent<Rigidbody2D>();

        if (homingType == HomingType.Slash)
        {
            StartCoroutine(SlashPattern());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (homingType == HomingType.Constant)
        {
            ConstantHoming();
        }
        else if (homingType == HomingType.Slash && isSlashing)
        {
            rb.linearVelocity = transform.up * speed * slashSpeedMultiplier;
        }
    }
    private new void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 normal = other.contacts[0].normal;
        Debug.Log("Homing knife collsion dot product " + Vector2.Dot(other.contacts[0].point, transform.up));
        Debug.DrawRay(rb.position, transform.up * 2f, Color.blue, 1f); // Knife's forward
        Debug.DrawRay(other.contacts[0].point.normalized, normal.normalized * 1f, Color.green, 1f); // Collision normal
        if (Vector2.Dot(normal, transform.up) < -0.5f)
        {
            base.OnCollisionEnter2D(other);
        }
    }

    private void ConstantHoming()
    {
        float rotateAmount = HomingRotation();
        rb.angularVelocity = rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.up * speed;
    }

    IEnumerator SlashPattern()
    {
        while (true)
        {
            float lockTimer = 0f;
            while (lockTimer < lockOnDuration)
            {
                float rotateAmount = HomingRotation();
                rb.angularVelocity = rotateAmount * rotateSpeed;
                rb.linearVelocity = Vector2.zero;

                lockTimer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // first slash
            isSlashing = true;
            rb.angularVelocity = 0f;
            yield return new WaitForSeconds(slashDuration);

            isSlashing = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = HomingRotation() * rotateSpeed ;
            yield return new WaitForSeconds(pauseDuration);

            // second slash
            isSlashing = true;
            rb.angularVelocity = 0f;
            yield return new WaitForSeconds(slashDuration);

            isSlashing = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
    private float HomingRotation()
    {
        Vector2 direction = (Vector2)target.position - rb.position;
        direction += offset;
        direction.Normalize();
        float rotateAmount = - Vector3.Cross(direction, transform.up).z;
        return rotateAmount;
    }
}
