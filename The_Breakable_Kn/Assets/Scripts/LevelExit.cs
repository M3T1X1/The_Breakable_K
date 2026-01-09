using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextSceneName = "Boss_Scene"; // Nazwa Twojej sceny z bossem

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Sprawdzamy czy to gracz
        if (other.CompareTag("Player"))
        {
            // 2. Liczymy ilu wrogów z tagiem "Enemy" zosta³o na mapie
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                // Jeœli 0, ³adujemy arenê bosa
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                // Jeœli wrogowie ¿yj¹, nic siê nie dzieje (mo¿esz dodaæ tekst)
                Debug.Log("Jeszcze ¿yje: " + enemies.Length + " wrogów!");
            }
        }
    }
}