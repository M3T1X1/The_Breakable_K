using UnityEngine;

public class BanditAI : MonoBehaviour
{
    // === Ustawienia ruchu ===
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolDistance = 5f;

    // === Ustawienia wykrywania ===
    public float lookRadius = 7f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    // === Zmienne techniczne ===
    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTarget;
    private Vector3 startPosition;
    private bool facingRight = true;
    private bool isAttacking = false;

    // === Parametry Animatora Bandyty (poprawne u¿ycie AnimState) ===
    private const string ANIM_STATE = "AnimState";
    private const int STATE_IDLE = 0;
    private const int STATE_RUN = 1;
    private const string ANIM_ATTACK = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        // ZnajdŸ gracza (tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    // =================================================================
    // PE£NA I POPRAWNA LOGIKA UPDATE()
    // =================================================================
    void Update()
    {
        if (playerTarget == null) return;
        if (rb == null) return;

        float distanceToPlayer = Mathf.Abs(playerTarget.position.x - transform.position.x);

        // 1. Sprawdzenie, czy gracz jest w zasiêgu wzroku (Look Radius)
        if (distanceToPlayer <= lookRadius)
        {
            // A. Czy gracz jest w zasiêgu ataku?
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            // B. Gracz jest widoczny, ale za daleko, wiêc go goñ
            else
            {
                ChasePlayer();
            }
        }
        // 2. Gracz jest za daleko, Bandyta patrolowuje
        else
        {
            Patrol();
        }
    }
    // =================================================================

    void Patrol()
    {
        // Blokada: Jeœli Bandyta jest zajêty atakiem, nie ruszaj siê
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetInteger(ANIM_STATE, STATE_IDLE);
            return;
        }

        // Sprawdzenie, czy doszed³ do granicy patrolowania
        bool boundaryReached = (transform.position.x >= startPosition.x + patrolDistance && facingRight) ||
                               (transform.position.x <= startPosition.x - patrolDistance && !facingRight);

        if (boundaryReached)
        {
            Flip();
        }

        // Ruch
        float direction = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
        anim.SetInteger(ANIM_STATE, STATE_RUN);
    }

    void ChasePlayer()
    {
        // Blokada: Jeœli Bandyta jest zajêty atakiem, nie goñ
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetInteger(ANIM_STATE, STATE_IDLE);
            return;
        }

        float direction = (playerTarget.position.x > transform.position.x) ? 1f : -1f;

        // Odwrócenie
        if ((direction > 0 && !facingRight) || (direction < 0 && facingRight))
        {
            Flip();
        }

        // Ruch
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        anim.SetInteger(ANIM_STATE, STATE_RUN);
    }

    void AttackPlayer()
    {
        if (isAttacking) return;

        isAttacking = true;

        // Bandyta siê zatrzymuje do ataku
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetInteger(ANIM_STATE, STATE_IDLE);

        // Aktywacja animacji ataku
        anim.SetTrigger(ANIM_ATTACK);

        // Cooldown
        Invoke("ResetAttack", attackCooldown);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Funkcja resetuj¹ca flagê ataku po cooldownie
    void ResetAttack()
    {
        isAttacking = false;
    }
}