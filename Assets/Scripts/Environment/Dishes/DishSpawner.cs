using System.Collections;
using UnityEngine;

/// <summary>
/// Dish spawner.
/// - Spawns random dishes (mugs, plates, bowls) at random intervals above the sink.
/// - Each spawned prefab can have its own controller (MugController, PlateController, BowlController)
///   which receives a random sprite.
/// - Stops spawning after reaching the configured maxDishes.
/// </summary>
public class DishSpawner : MonoBehaviour
{
    [Header("Dish Prefabs")]
    [Tooltip("Assign your dish prefabs here (mug prefab should contain the MugController).")]
    public GameObject[] dishes; // Mug, Plate, Bowl prefabs

    [Header("Mug visuals")]
    public Sprite[] mugSprites;

    [Header("Plate visuals")]
    public Sprite[] plateSprites;

    [Header("Bowl visuals")]
    public Sprite[] bowlSprites;

    [Header("Spawn Settings")]
    public Transform spawnPoint;      // empty object above sink - where dishes spawn
    public Transform spawnParent;     // optional parent for organisation
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    [Tooltip("Maximum number of dishes to spawn before stopping.")]
    public int maxDishes = 8;

    [Header("Dish Physics")]
    public float fallSpeed = 5f;

    // internal count of how many dishes have spawned
    private int dishesSpawned = 0;

    private void Start()
    {
        if (dishes == null || dishes.Length == 0)
            Debug.LogWarning("[DishSpawner] No dish prefabs assigned.");

        StartCoroutine(SpawnDishesRoutine());
    }

    private IEnumerator SpawnDishesRoutine()
    {
        while (dishesSpawned < maxDishes)
        {
            // pick a random dish prefab
            GameObject dishPrefab = dishes[Random.Range(0, dishes.Length)];

            if (dishPrefab == null)
            {
                Debug.LogWarning("[DishSpawner] Selected dish prefab was null. Skipping spawn.");
            }
            else
            {
                // spawn it at spawnPoint
                Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
                GameObject dish = Instantiate(dishPrefab, spawnPos, Quaternion.identity, spawnParent);

                // assign visuals depending on dish type
                MugController mug = dish.GetComponent<MugController>();
                if (mug != null && mugSprites != null && mugSprites.Length > 0)
                {
                    Sprite chosen = mugSprites[Random.Range(0, mugSprites.Length)];
                    mug.SetSprite(chosen);
                }

                PlateController plate = dish.GetComponent<PlateController>();
                if (plate != null && plateSprites != null && plateSprites.Length > 0)
                {
                    Sprite chosen = plateSprites[Random.Range(0, plateSprites.Length)];
                    plate.SetSprite(chosen);
                }

                BowlController bowl = dish.GetComponent<BowlController>();
                if (bowl != null && bowlSprites != null && bowlSprites.Length > 0)
                {
                    Sprite chosen = bowlSprites[Random.Range(0, bowlSprites.Length)];
                    bowl.SetSprite(chosen);
                }

                // apply downward velocity
                Rigidbody2D rb = dish.GetComponent<Rigidbody2D>();
                if (rb != null && fallSpeed > 0f)
                {
                    rb.linearVelocity = Vector2.down * fallSpeed;
                }

                // increment counter
                dishesSpawned++;
            }

            // wait a random time before next spawn (unless we've hit the max)
            if (dishesSpawned < maxDishes)
            {
                float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(waitTime);
            }
        }

        Debug.Log("[DishSpawner] Finished spawning dishes.");
    }
}
