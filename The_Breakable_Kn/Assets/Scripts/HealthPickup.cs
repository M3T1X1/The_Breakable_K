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
                // Teraz ju¿ widzi maxArmorPoints!
                if (playerHealth.armorPoints < playerHealth.maxArmorPoints)
                {
                    playerHealth.AddArmor(amount);
                    Destroy(gameObject);
                }
            }
        }
    }
}