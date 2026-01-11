using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // UWAGA: armorPoints jest statyczne, wiêc PlayerHealth.armorPoints
                // maxArmorPoints NIE jest statyczne, wiêc playerHealth.maxArmorPoints
                if (PlayerHealth.armorPoints < playerHealth.maxArmorPoints)
                {
                    playerHealth.AddArmor(amount);
                    Destroy(gameObject);
                }
            }
        }
    }
}