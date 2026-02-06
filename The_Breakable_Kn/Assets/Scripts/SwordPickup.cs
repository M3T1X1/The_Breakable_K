using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public int amount = 5; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                ItemPersistence persistence = GetComponent<ItemPersistence>();
                if (persistence != null)
                {
                    persistence.MarkAsCollected();
                }

                playerHealth.AddSwordPower(amount);
                Destroy(gameObject);
            }
        }
    }
}