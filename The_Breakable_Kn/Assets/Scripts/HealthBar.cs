using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage; // Przeci¹gniesz tu obrazek "Fill"

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // Obliczamy procent ¿ycia (od 0 do 1)
        float fillAmount = (float)currentHealth / maxHealth;
        fillImage.fillAmount = fillAmount;
    }
}