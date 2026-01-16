using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextSceneName = "Boss_Scene"; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("Jeszcze ¿yje: " + enemies.Length + " wrogów!");
            }
        }
    }
}