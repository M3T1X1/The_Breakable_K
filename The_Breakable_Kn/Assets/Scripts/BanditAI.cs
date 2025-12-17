using UnityEngine;

public class BanditAI : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ustawienia walki")]
    public float lookRadius = 7f;
    public float attackRange = 1.8f; // Bliski zasięg (jak wcześniej)
    public float attackCooldown = 1.5f;
    public int damageAmount = 10;

    public Transform attackPoint;    // Puste miejsce przed Magiem
    public float attackArea = 0.5f;  // Wielkość zasięgu ciosu
    public LayerMask playerLayer;    // Ustaw na "Player"

    [Header("Granice")]
    public Transform lewaGranica;
    public Transform prawaGranica;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTarget;
    private bool isAttacking = false;
    private bool idzieWPrawo = true;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void Update()
    {
        // 1. Podstawowe sprawdzenie czy obiekty istnieją
        if (playerTarget == null || rb == null) return;

        // 2. NOWOŚĆ: Sprawdzenie czy gracz już nie żyje
        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            StopMoving(); // Mag przestaje chodzić
            return;       // Wychodzimy z funkcji, mag nic więcej nie robi
        }

        // 3. Sprawdzenie czy mag żyje (Twoje poprzednie)
        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().currentHealth <= 0) return;

        // --- Reszta Twojego kodu bez zmian ---
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= lookRadius)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }
                else if (!isAttacking)
                {
                    StopMoving();
                }
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        StopMoving();

        // Odpalamy animację ataku (upewnij się, że w Animatorze masz Trigger "Attack")
        anim.SetTrigger("Attack");

        // Wywołujemy zadanie obrażeń z opóźnieniem, żeby pasowało do machnięcia ręką
        Invoke("DealDamage", 0.4f);
        Invoke("ResetAttack", attackCooldown);
    }

    void DealDamage()
    {
        // Zabezpieczenie: jeśli zapomniałeś przypisać attackPoint w Inspektorze
        if (attackPoint == null)
        {
            Debug.LogError("Mag nie ma przypisanego AttackPoint!");
            return;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackArea, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            // Szukamy skryptu PlayerHealth
            PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 0);
    }

    // --- RUCH (PATROL I POGOŃ) ---

    void Patrol()
    {
        if (idzieWPrawo && transform.position.x >= prawaGranica.position.x) idzieWPrawo = false;
        else if (!idzieWPrawo && transform.position.x <= lewaGranica.position.x) idzieWPrawo = true;

        transform.localScale = idzieWPrawo ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2((idzieWPrawo ? 1f : -1f) * walkSpeed, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 1);
    }

    void ChasePlayer()
    {
        if (isAttacking) return;
        float direction = (playerTarget.position.x > transform.position.x) ? 1f : -1f;

        if ((direction > 0 && transform.position.x >= prawaGranica.position.x) ||
            (direction < 0 && transform.position.x <= lewaGranica.position.x))
        {
            StopMoving();
            return;
        }

        transform.localScale = (direction > 0) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 1);
    }

    // Rysowanie zasięgu ataku w edytorze
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackArea);
    }
}