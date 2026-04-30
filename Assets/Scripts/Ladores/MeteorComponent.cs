using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Vector3 targetImpactPoint;
    private float damage;
    private float explosionRadius;
    private GameObject explosionEffect;
    private MeteorStrikeManager manager;
    private float speed = 25f;
    private Rigidbody rb;
    private bool hasHit = false;

    [Header("Meteor Visuals")]
    [SerializeField] ParticleSystem trailParticles;
    [SerializeField] Light meteorLight;

    public void Initialize(Vector3 target, float dmg, float radius, GameObject explosion, MeteorStrikeManager strikeManager)
    {
        targetImpactPoint = target;
        damage = dmg;
        explosionRadius = radius;
        explosionEffect = explosion;
        manager = strikeManager;

        // Setup rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.mass = 100f;

        // Calculate direction and speed
        Vector3 direction = (targetImpactPoint - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Optional: Add random rotation
        rb.angularVelocity = Random.insideUnitSphere * 5f;

        // Auto-destroy if it misses after 10 seconds
        Destroy(gameObject, 10f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        // Deal damage to players
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            // Look for your Character script (or Health component)
            Character character = hit.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(damage);
                Debug.Log($"Meteor hit {hit.name} for {damage} damage!");
            }

            // Alternative: If you have a separate Health script
            // Health health = hit.GetComponent<Health>();
            // if (health != null) health.TakeDamage(damage);
        }

        // Spawn explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        // Trigger camera shake
        if (manager != null)
        {
            manager.TriggerCameraShake();
        }

        // Remove from manager and destroy meteor
        if (manager != null)
        {
            manager.RemoveMeteor(gameObject);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}