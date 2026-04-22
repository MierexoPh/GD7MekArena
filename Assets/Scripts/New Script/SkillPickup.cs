using UnityEngine;

public class SkillPickup : MonoBehaviour
{
    public SkillSystem.SkillType skillType;

    [Header("Spawn Settings")]
    public float rotateSpeed = 90f;
    [HideInInspector] public float lifetime = 5f;
    float lifetimeTimer = 0f;

    void Start()
    {
        skillType = (SkillSystem.SkillType)Random.Range(0, 3);
        Debug.Log("Pickup spawned with skill: " + skillType);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // countdown lifetime, destroy if not picked up
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Debug.Log("Pickup expired, respawning...");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        SkillSystem skillSystem = other.GetComponent<SkillSystem>();
        if (skillSystem != null)
        {
            skillSystem.AssignSkill(skillType);
            Destroy(gameObject);
        }
    }
}