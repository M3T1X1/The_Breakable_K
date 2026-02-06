using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public HashSet<string> defeatedEnemies = new HashSet<string>();

    public HashSet<string> collectedItems = new HashSet<string>();

    public Vector3 playerSpawnPos;
    public string lastSceneName;
    public bool useSpawnPos = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearData()
    {
        defeatedEnemies.Clear();
        collectedItems.Clear(); 
        useSpawnPos = false;
    }
}