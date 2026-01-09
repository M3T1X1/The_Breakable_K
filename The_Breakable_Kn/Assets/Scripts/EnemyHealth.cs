using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false; // To zatrzyma wielokrotne odpalanie animacji œmierci

    [Header("UI")]
    public HealthBar healthBar;

    [Header("Odpornoœæ")]
    public float damageCooldown = 0.5f;
    private float lastDamageTime;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        // Jeœli ju¿ nie ¿yje, natychmiast wychodzimy - to naprawi skakanie przy œmierci
        if (currentHealth <= 0) return;

        if (Time.time < lastDamageTime + damageCooldown) return;

        lastDamageTime = Time.time;
        currentHealth -= damage;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Wykonaj animacjê oberwania tylko jeœli Król prze¿y³ cios
            if (anim != null) anim.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.ResetTrigger("Hurt");
        anim.SetTrigger("Death");

        if (healthBar != null) healthBar.gameObject.SetActive(false);

        if (GetComponent<BanditAI>() != null) GetComponent<BanditAI>().enabled = false;
        if (GetComponent<KingBossAI>() != null) GetComponent<KingBossAI>().enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // --- NOWA LOGIKA ZWYCIÊSTWA ---
        // Sprawdzamy, czy ten wróg nazywa siê KROL
        if (gameObject.name == "Krol")
        {
            // Szukamy gracza i wywo³ujemy now¹ funkcjê Win()
            PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                player.Win();
            }
        }
        // ------------------------------

        Destroy(gameObject, 4f);
    }
}