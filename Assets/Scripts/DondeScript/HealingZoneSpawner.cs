using UnityEngine;

public class HealingZoneSpawner : MonoBehaviour
{
    public GameObject healingZonePrefab;

    [Header("Spawn Settings")]
    public Vector3 spawnCenter = Vector3.zero;
    public float spawnRadius = 10f;
    public float spawnInterval = 10f;

    private GameObject currentZone;

    void Start()
    {
        InvokeRepeating(nameof(SpawnHealingZone), 1f, spawnInterval);
    }

    void SpawnHealingZone()
    {
        // Prevent piling up (only 1 exists at a time)
        if (currentZone != null)
        {
            Destroy(currentZone);
        }

        // Random circular offset
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = spawnCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        currentZone = Instantiate(healingZonePrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Healing Zone Spawned at: " + spawnPosition);
    }
}