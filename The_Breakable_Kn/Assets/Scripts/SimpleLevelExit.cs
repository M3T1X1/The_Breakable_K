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
                    GameManager.instance.useSpawnPos = true;
                }
                else
                {
                    GameManager.instance.playerSpawnPos = transform.position + new Vector3(0, -2f, 0);
                    GameManager.instance.useSpawnPos = false;
                }
            }
            SceneManager.LoadScene(nextSceneName);
        }
    }
}