using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public enum SkillType { DoubleTeam, Invisibility, BigAmmo }

    [Header("Skill Settings")]
    public SkillType assignedSkill;
    [SerializeField] float skillDuration = 5f;
    [SerializeField] float skillCooldown = 10f;

    float skillCooldownTimer = 0f;
    bool skillActive = false;
    float skillTimer = 0f;
    bool hasSkill = false;

    GameObject clone;
    Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    public void AssignSkill(SkillType skill)
    {
        assignedSkill = skill;
        hasSkill = true;
        skillCooldownTimer = 0f;
        Debug.Log((character.isPlayer2 ? "Player2" : "Player1") + " picked up skill: " + assignedSkill);
    }

    void Update()
    {
        if (!hasSkill) return;

        if (!character.isPlayer2)
        {
            if (Input.GetKeyDown(KeyCode.H))
                ActivateSkill();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Keypad3))
                ActivateSkill();
        }

        if (skillActive)
        {
            skillTimer += Time.deltaTime;
            if (skillTimer >= skillDuration)
                DeactivateSkill();
        }

        if (skillCooldownTimer > 0)
            skillCooldownTimer -= Time.deltaTime;
    }

    void ActivateSkill()
    {
        if (skillActive || skillCooldownTimer > 0) return;

        skillActive = true;
        skillTimer = 0f;

        switch (assignedSkill)
        {
            case SkillType.DoubleTeam:
                Vector3 spawnPos = transform.position + transform.right * 2f;
                clone = Instantiate(this.gameObject, spawnPos, transform.rotation);

                // Disable player input and skill on clone
                clone.GetComponent<Character>().enabled = false;
                clone.GetComponent<CharacterController>().enabled = false;
                clone.GetComponent<SkillSystem>().enabled = false;

                // Add clone AI to attack
                CloneAI cloneAI = clone.AddComponent<CloneAI>();
                cloneAI.owner = this.gameObject;
                cloneAI.isOwnerPlayer2 = character.isPlayer2;

                Debug.Log("Double Team activated!");
                break;

            case SkillType.Invisibility:
                GetComponentInChildren<Renderer>().enabled = false;
                Debug.Log("Invisibility activated!");
                break;

            case SkillType.BigAmmo:
                character.bulletDamage *= 2f;
                character.bulletSpeed *= 0.5f;
                Debug.Log("Big Ammo activated!");
                break;
        }
    }

    void DeactivateSkill()
    {
        skillActive = false;
        skillCooldownTimer = skillCooldown;
        hasSkill = false;

        switch (assignedSkill)
        {
            case SkillType.DoubleTeam:
                if (clone != null) Destroy(clone);
                break;

            case SkillType.Invisibility:
                GetComponentInChildren<Renderer>().enabled = true;
                break;

            case SkillType.BigAmmo:
                character.bulletDamage /= 2f;
                character.bulletSpeed *= 2f;
                break;
        }
    }

    public bool IsSkillReady() => skillCooldownTimer <= 0 && !skillActive;
    public bool IsSkillActive() => skillActive;
    public float GetCooldownRemaining() => skillCooldownTimer;
}