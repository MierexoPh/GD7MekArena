using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections;

public class MeteorStrikeManager : MonoBehaviour
{
    [Header("Meteor Settings")]
    [SerializeField] GameObject meteorPrefab;
    [SerializeField] int maxMeteorsAtOnce = 5;
    [SerializeField] float meteorDamage = 40f;
    [SerializeField] float meteorRadius = 4f;

    [Header("Spawn Settings")]
    [SerializeField] float minSpawnDelay = 5f;
    [SerializeField] float maxSpawnDelay = 15f;
    [SerializeField] float spawnHeight = 20f;
    [SerializeField] float spawnWidth = 25f;  // How wide the spawn area is
    [SerializeField] float spawnLength = 25f; // How long the spawn area is

    [Header("Impact Effects")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float cameraShakeDuration = 0.3f;
    [SerializeField] float cameraShakeMagnitude = 0.2f;

    [Header("Warning System")]
    [SerializeField] GameObject warningIndicatorPrefab;
    [SerializeField] float warningTime = 1.5f;

    private List<GameObject> activeMeteors = new List<GameObject>();
    private Camera mainCamera;
    private Vector3 spawnAreaCenter = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(MeteorSpawnRoutine());

        // Set spawn area center (can be manually set or auto-detected)
        spawnAreaCenter = transform.position;

        Debug.Log("Meteor Strike Manager initialized. Meteors will start falling soon!");
    }

    IEnumerator MeteorSpawnRoutine()
    {
        while (true)
        {
            // Wait random time between meteor strikes
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            // Spawn a meteor if we haven't reached the max
            if (activeMeteors.Count < maxMeteorsAtOnce)
            {
                SpawnRandomMeteor();
            }
        }
    }

    void SpawnRandomMeteor()
    {
        // Random X and Z position within the spawn area
        float randomX = spawnAreaCenter.x + Random.Range(-spawnWidth / 2, spawnWidth / 2);
        float randomZ = spawnAreaCenter.z + Random.Range(-spawnLength / 2, spawnLength / 2);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        // Calculate impact position
        Vector3 impactPosition = new Vector3(randomX, 0, randomZ);

        // Show warning indicator if prefab assigned
        if (warningIndicatorPrefab != null)
        {
            StartCoroutine(ShowWarningAndSpawn(impactPosition, spawnPosition));
        }
        else
        {
            SpawnMeteorDirect(spawnPosition, impactPosition);
        }
    }

    IEnumerator ShowWarningAndSpawn(Vector3 impactPosition, Vector3 spawnPosition)
    {
        // Show UI warning
        if (UIWarningSystem.Instance != null)
        {
            UIWarningSystem.Instance.ShowMeteorWarning();
        }

        // Wait for warning duration
        yield return new WaitForSeconds(warningTime);

        // Spawn the meteor
        SpawnMeteorDirect(spawnPosition, impactPosition);
    }

    void SpawnMeteorDirect(Vector3 spawnPosition, Vector3 impactPosition)
    {
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        activeMeteors.Add(meteor);

        // Get meteor component and setup
        Meteor meteorScript = meteor.GetComponent<Meteor>();
        if (meteorScript == null)
        {
            meteorScript = meteor.AddComponent<Meteor>();
        }

        meteorScript.Initialize(impactPosition, meteorDamage, meteorRadius, explosionPrefab, this);
    }

    public void RemoveMeteor(GameObject meteor)
    {
        if (activeMeteors.Contains(meteor))
        {
            activeMeteors.Remove(meteor);
        }
    }

    public void TriggerCameraShake()
    {
        if (mainCamera != null)
        {
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            float y = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize spawn area in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, 1, spawnLength));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, spawnHeight, transform.position.z),
                            new Vector3(spawnWidth, 1, spawnLength));
    }
}