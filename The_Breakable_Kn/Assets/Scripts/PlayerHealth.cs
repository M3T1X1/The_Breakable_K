using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Statystyki Startowe")]
    public int heartHealth = 2;
    public int armorPoints = 0;

    // Zmienione na public, ¿eby skrypt he³mu widzia³ te limity
    [HideInInspector] public int maxHeartHealth = 2;
    [HideInInspector] public int maxArmorPoints = 3;

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
        heartHealth = Mathf.Clamp(heartHealth, 0, maxHeartHealth);
        armorPoints = Mathf.Clamp(armorPoints, 0, maxArmorPoints);
        UpdateUI();
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (heartHealth < maxHeartHealth)
        {
            heartHealth += amount;
            heartHealth = Mathf.Clamp(heartHealth, 0, maxHeartHealth);
            UpdateUI();
        }
    }

    public void AddArmor(int amount)
    {
        if (isDead) return;
        if (armorPoints < maxArmorPoints)
        {
            armorPoints += amount;
            armorPoints = Mathf.Clamp(armorPoints, 0, maxArmorPoints);
            UpdateUI();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        for (int i = 0; i < damage; i++)
        {
            if (armorPoints > 0) armorPoints--;
            else heartHealth--;
        }
        heartHealth = Mathf.Clamp(heartHealth, 0, maxHeartHealth);
        UpdateUI();
        if (heartHealth <= 0) Die();
        else if (GetComponent<Animator>() != null) GetComponent<Animator>().SetTrigger("Hurt");
    }

    void UpdateUI()
    {
        if (contentContainer == null || heartPrefab == null || helmPrefab == null) return;
        foreach (GameObject icon in spawnedIcons) Destroy(icon);
        spawnedIcons.Clear();

        if (heartHealth > 0)
        {
            GameObject h = Instantiate(heartPrefab, contentContainer);
            Image heartImg = h.GetComponent<Image>();
            if (heartImg != null) heartImg.sprite = (heartHealth >= 2) ? fullHeart : halfHeart;
            spawnedIcons.Add(h);
        }

        for (int i = 0; i < armorPoints; i++)
        {
            GameObject helm = Instantiate(helmPrefab, contentContainer);
            spawnedIcons.Add(helm);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (GetComponent<Animator>() != null) GetComponent<Animator>().SetTrigger("Death");
        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = false;
        if (gameOverScreen != null) { gameOverScreen.SetActive(true); Invoke("ShowButton", 5f); }
    }

    void ShowButton() { if (restartButton != null) restartButton.SetActive(true); }
    public void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    public void Win()
    {
        // U¿ywamy tych samych obiektów co przy œmierci (gameOverScreen to Twój PlayerHUD)
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);

            // Szukamy tekstu w PlayerHUD i zmieniamy go na z³ote zwyciêstwo
            TMPro.TextMeshProUGUI statusText = gameOverScreen.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (statusText != null)
            {
                statusText.text = "ZAJEBA£EŒ SKURWYSYNA!";
                statusText.color = new Color(1f, 0.84f, 0f); // Z³oty kolor
            }

            // Pokazujemy przycisk restartu
            Invoke("ShowButton", 2f);
        }
    }

}