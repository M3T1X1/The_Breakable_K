using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Lista ID pokonanych wrogów
    public HashSet<string> defeatedEnemies = new HashSet<string>();

    // NOWE: Lista ID zebranych przedmiotów (miecze, helmy)
    public HashSet<string> collectedItems = new HashSet<string>();

    // Zapamiêtana pozycja i scena
    public Vector3 playerSpawnPos;
    public string lastSceneName;
    public bool useSpawnPos = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // To sprawia, ¿e obiekt nie znika po zmianie sceny
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funkcja czyszcz¹ca wszystko (np. przy ca³kowitym restarcie gry)
    public void ClearData()
    {
        defeatedEnemies.Clear();
        collectedItems.Clear(); // Czyœcimy te¿ listê przedmiotów
        useSpawnPos = false;
    }
}