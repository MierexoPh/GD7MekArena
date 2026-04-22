using UnityEngine;

public class SkillPickupSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject skillPickupPrefab;

    [Header("Map Bounds")]
    public float mapMinX = -20f;
    public float mapMaxX = 20f;
    public float mapMinZ = -20f;
    public float mapMaxZ = 20f;
    public float spawnY = 1f;

    [Header("Pickup Timer")]
    public float pickupLifetime = 5f; // destroy and respawn if not picked up

    GameObject currentPickup;

    void Start()
    {
        SpawnPickup();
    }

    void Update()
    {
        // if pickup was collected or destroyed, spawn a new one
        if (currentPickup == null)
        {
            SpawnPickup();
        }
    }

    void SpawnPickup()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(mapMinX, mapMaxX),
            spawnY,
            Random.Range(mapMinZ, mapMaxZ)
        );

        currentPickup = Instantiate(skillPickupPrefab, randomPos, Quaternion.identity);

        // Pass lifetime to the pickup so it destroys itself
        currentPickup.GetComponent<SkillPickup>().lifetime = pickupLifetime;
    }
}