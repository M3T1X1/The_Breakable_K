using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLevelExit : MonoBehaviour
{
    public string nextSceneName;

    [Header("Ustawienia powrotu")]
    public bool isReturnPortal = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                if (isReturnPortal)
                {
                    // Jeœli wracamy z nieba
                    GameManager.instance.useSpawnPos = true;
                }
                else
                {
                    // Teleportujemy Ciê 10 jednostek w lewo i 2 jednostki wy¿ej wzglêdem portalu
                    // To powinno Ciê postawiæ nad zupe³nie inn¹ platform¹
                    // Ustawiamy X na 0 (pod portalem), a Y na -2.5f (bezpiecznie pod triggerem)
                    GameManager.instance.playerSpawnPos = transform.position + new Vector3(0, -2f, 0);
                    GameManager.instance.useSpawnPos = false;
                }
            }
            SceneManager.LoadScene(nextSceneName);
        }
    }
}