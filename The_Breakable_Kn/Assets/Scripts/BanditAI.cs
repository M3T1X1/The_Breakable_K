using UnityEngine;

public class BanditAI : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ustawienia walki")]
    public float lookRadius = 7f;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    public int damageAmount = 10;

    public Transform attackPoint;
    public float attackArea = 0.5f;
    public LayerMask playerLayer;

    [Header("Granice")]
    public Transform lewaGranica;
    public Transform prawaGranica;

    // --- NOWE ZMIENNE DO ZAPAMIĘTANIA POZYCJI ---
    private float pozycjaLewejGranicy;
    private float pozycjaPrawejGranicy;

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

       
        if (lewaGranica != null && prawaGranica != null)
        {
            pozycjaLewejGranicy = lewaGranica.position.x;
            pozycjaPrawejGranicy = prawaGranica.position.x;
        }
        else
        {
            Debug.LogError("Mag o nazwie " + gameObject.name + " nie ma przypisanych granic w Inspektorze!");
        }
    }

    void Update()
    {
        if (playerTarget == null || rb == null) return;

        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            StopMoving();
            return;
        }

        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().currentHealth <= 0) return;

        // 1. Obliczamy odległość ogólną (koło)
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // Używamy bounds.center zamiast transform.position.y
        float heightDifference = Mathf.Abs(GetComponent<Collider2D>().bounds.center.y - playerTarget.GetComponent<Collider2D>().bounds.center.y);

        // 3. Dodajemy warunek: Musisz być w zasięgu wzroku ORAZ blisko na wysokość (np. mniej niż 2 jednostki)
        if (distanceToPlayer <= lookRadius && heightDifference < 1.4f)
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
            // Jeśli jesteś za wysoko lub za daleko, Mag wraca do patrolu
            Patrol();
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        StopMoving();
        anim.SetTrigger("Attack");
        Invoke("DealDamage", 0.4f);
        Invoke("ResetAttack", attackCooldown);
    }

    void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackArea, playerLayer);
        foreach (Collider2D playerCollider in hitPlayers)
        {
            PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(1);
        }
    }

    void ResetAttack() { isAttacking = false; }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 0);
    }

    void Patrol()
    {
        if (idzieWPrawo && transform.position.x >= pozycjaPrawejGranicy) idzieWPrawo = false;
        else if (!idzieWPrawo && transform.position.x <= pozycjaLewejGranicy) idzieWPrawo = true;

        transform.localScale = idzieWPrawo ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2((idzieWPrawo ? 1f : -1f) * walkSpeed, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 1);
    }

    void ChasePlayer()
    {
        if (isAttacking) return;
        float direction = (playerTarget.position.x > transform.position.x) ? 1f : -1f;

        if ((direction > 0 && transform.position.x >= pozycjaPrawejGranicy) ||
            (direction < 0 && transform.position.x <= pozycjaLewejGranicy))
        {
            StopMoving();
            return;
        }

        transform.localScale = (direction > 0) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 1);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackArea);
    }
}