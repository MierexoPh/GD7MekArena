using UnityEngine;

public class CloneAI : MonoBehaviour
{
    [HideInInspector] public GameObject owner;
    [HideInInspector] public bool isOwnerPlayer2;

    Transform target;
    float detectionRange = 20f;
    float fireRate = 0.5f;
    float fireRateTimer = 0f;

    Character ownerCharacter;
    Transform cloneFirePoint; // clone's own fire point

    void Start()
    {
        ownerCharacter = owner.GetComponent<Character>();

        // Get the clone's own firepoint
        cloneFirePoint = transform.Find(ownerCharacter.firePoint.name);

        // Tint clone so its visually different
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.material.color = Color.cyan;
    }

    void Update()
    {
        DetectTarget();

        fireRateTimer += Time.deltaTime;

        if (target != null && fireRateTimer >= fireRate)
        {
            ShootAtTarget();
            fireRateTimer = 0f;
        }

        // Face target
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void DetectTarget()
    {
        string enemyLayer = isOwnerPlayer2 ? "Player1" : "Player2";

        Collider[] hit = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask(enemyLayer));
        if (hit.Length > 0)
            target = hit[0].transform;
        else
            target = null;
    }

    void ShootAtTarget()
    {
        if (ownerCharacter.basicBullet == null || cloneFirePoint == null) return;

        // Shoot from clone's own fire point, not the owner's
        GameObject bullet = Instantiate(ownerCharacter.basicBullet, cloneFirePoint.position, cloneFirePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetTarget(target, ownerCharacter.bulletDamage, ownerCharacter.bulletSpeed, ownerCharacter.bulletTurnSpeed);
    }
}