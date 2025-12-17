using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public HealthBar healthBar; // Tu przeci¹gniesz skrypt HealthBar

    [Header("Odpornoœæ")]
    public float damageCooldown = 0.5f; // Mag mo¿e dostaæ obra¿enia tylko raz na pó³ sekundy
    private float lastDamageTime;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Ustawiamy pasek na pe³ny na starcie
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        // Jeœli od ostatniego ciosu minê³o za ma³o czasu - zignoruj atak gracza
        if (Time.time < lastDamageTime + damageCooldown) return;

        lastDamageTime = Time.time;
        currentHealth -= damage;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
        if (anim != null) anim.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (anim != null) anim.SetTrigger("Death");

        // Ukrywamy pasek ¿ycia po œmierci
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        if (GetComponent<BanditAI>() != null) GetComponent<BanditAI>().enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        Destroy(gameObject, 2f);
    }
}