using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Statystyki Startowe")]
    public static int heartHealth = 2;
    public static int armorPoints = 0;

    [Header("Miecz - Bonus")]
    public static int swordCharges = 0; 
    public int boostedDamage = 40;
    public int normalDamage = 20;

    [Header("Limity")]
    public int maxHeartHealth = 2;
    public int maxArmorPoints = 3;

    public bool isDead = false;

    [Header("UI Referencje - Statystyki")]
    public Transform contentContainer; 
    public GameObject heartPrefab;
    public GameObject helmPrefab;

    [Header("UI Referencje - Bonusy")]
    public Transform weaponContainer; 
    public GameObject swordIconPrefab; 

    [Header("Grafiki")]
    public Sprite fullHeart;
    public Sprite halfHeart;

    [Header("Koniec Gry - Twój Pergamin")]
    public GameObject endGameScreen;

    private List<GameObject> spawnedIcons = new List<GameObject>();
    private List<GameObject> spawnedBuffs = new List<GameObject>();

    void Start()
    {
        heartHealth = Mathf.Clamp(heartHealth, 0, maxHeartHealth);
        armorPoints = Mathf.Clamp(armorPoints, 0, maxArmorPoints);

        if (endGameScreen != null) endGameScreen.SetActive(false);

        UpdateUI();
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

    public void AddSwordPower(int charges)
    {
        if (isDead) return;
        swordCharges += charges;
        UpdateUI();
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

    public void UpdateUI()
    {
        if (contentContainer != null && heartPrefab != null && helmPrefab != null)
        {
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

        if (weaponContainer != null && swordIconPrefab != null)
        {
            foreach (GameObject buff in spawnedBuffs) Destroy(buff);
            spawnedBuffs.Clear();

            if (swordCharges > 0)
            {
                GameObject sUI = Instantiate(swordIconPrefab, weaponContainer);
                sUI.SetActive(true); 

                TextMeshProUGUI txt = sUI.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = swordCharges.ToString();

                spawnedBuffs.Add(sUI);
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GetComponent<Animator>() != null) GetComponent<Animator>().SetTrigger("Death");
        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = false;

        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);
            TextMeshProUGUI statusText = endGameScreen.GetComponentInChildren<TextMeshProUGUI>(true);
            if (statusText != null)
            {
                statusText.gameObject.SetActive(true);
                statusText.text = "WASTED";
                statusText.color = Color.red;
            }

            Button restartBtn = endGameScreen.GetComponentInChildren<Button>(true);
            if (restartBtn != null) restartBtn.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.defeatedEnemies.Clear();
            GameManager.instance.useSpawnPos = false;
        }

        armorPoints = 0;
        heartHealth = 2;
        swordCharges = 0;
        SceneManager.LoadScene("Start_Scene");
    }

    public void Win()
    {
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);

            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.bodyType = RigidbodyType2D.Kinematic; 
            }

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetInteger("AnimState", 0); 
            }

            TextMeshProUGUI statusText = endGameScreen.GetComponentInChildren<TextMeshProUGUI>(true);
            if (statusText != null)
            {
                statusText.gameObject.SetActive(true);
                statusText.text = "YOU WIN";

                statusText.color = new Color(1f, 0.6f, 0f);

                statusText.outlineColor = new Color(0, 0, 0, 1);
                statusText.outlineWidth = 0.25f;

                statusText.fontStyle = FontStyles.Bold;

                statusText.ForceMeshUpdate();
            }

            Button restartBtn = endGameScreen.GetComponentInChildren<Button>(true);
            if (restartBtn != null)
            {
                restartBtn.gameObject.SetActive(true);
            }
        }
    }
}