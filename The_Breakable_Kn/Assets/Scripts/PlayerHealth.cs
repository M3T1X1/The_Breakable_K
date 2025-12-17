using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Statystyki")]
    [Range(0, 2)] public int heartHealth = 2; // Suwak w inspektorze ograniczony do 2
    public int armorPoints = 0;
    public bool isDead = false;

    [Header("UI Referencje")]
    public Transform contentContainer;
    public GameObject heartPrefab;
    public GameObject helmPrefab;

    [Header("Grafiki")]
    public Sprite fullHeart;
    public Sprite halfHeart;

    [Header("Koniec Gry")]
    public GameObject gameOverScreen;
    public GameObject restartButton;

    private List<GameObject> spawnedIcons = new List<GameObject>();

    void Start()
    {
        // WYMUSZENIE LIMITÓW NA STARCIE:
        if (heartHealth > 2) heartHealth = 2;
        if (armorPoints > 3) armorPoints = 3; // DODAJ TO zabezpieczenie

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // Najpierw tracimy pancerz, potem serce
        if (armorPoints > 0)
        {
            armorPoints--;
        }
        else
        {
            heartHealth -= damage;
        }

        // ZABEZPIECZENIE: Upewniamy siê, ¿e zdrowie nie wyjdzie poza zakres 0-2
        heartHealth = Mathf.Clamp(heartHealth, 0, 2);

        UpdateUI();

        if (heartHealth <= 0)
        {
            Die();
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Hurt");
        }
    }

    public void AddArmor()
    {
        if (isDead) return;

        if (armorPoints < 3)
        {
            armorPoints++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Sprawdzamy czy mamy wszystko podpiête, ¿eby nie by³o b³êdów NullReference
        if (contentContainer == null || heartPrefab == null || helmPrefab == null) return;

        // Czyœcimy stare ikony
        foreach (GameObject icon in spawnedIcons) Destroy(icon);
        spawnedIcons.Clear();

        // 1. Rysowanie Serca (zawsze tylko jedno, bo max HP to 2)
        if (heartHealth > 0)
        {
            GameObject h = Instantiate(heartPrefab, contentContainer);
            // Wybór grafiki: 2 HP = Pe³ne, 1 HP = Po³owa
            Image heartImage = h.GetComponent<Image>();
            if (heartImage != null)
            {
                heartImage.sprite = (heartHealth >= 2) ? fullHeart : halfHeart;
            }
            spawnedIcons.Add(h);
        }

        // 2. Rysowanie He³mów (Pancerza)
        // Mathf.Min wybierze mniejsz¹ liczbê: albo to co masz, albo 3.
        int visualArmor = Mathf.Min(armorPoints, 3);

        for (int i = 0; i < visualArmor; i++)
        {
            GameObject helm = Instantiate(helmPrefab, contentContainer);
            spawnedIcons.Add(helm);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        GetComponent<Animator>().SetTrigger("Death");

        // Blokada ruchu
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        // Zatrzymanie fizyki
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Ekran koñca gry
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Invoke("ShowButton", 5f);
        }
    }

    void ShowButton()
    {
        if (restartButton != null) restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}