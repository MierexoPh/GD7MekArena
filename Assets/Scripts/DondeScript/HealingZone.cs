using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public float healPerSecond = 10f;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerStay(Collider other)
    {
        Character player = other.GetComponent<Character>();
        if (player != null)
        {
            player.Heal(healPerSecond * Time.deltaTime);
        }
    }
}