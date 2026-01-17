using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public int amount = 5; // Ile uderzeñ daje miecz

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Dodajemy ³adunki miecza
                playerHealth.AddSwordPower(amount);
                // Niszczymy mieczyk na ziemi
                Destroy(gameObject);
            }
        }
    }
}