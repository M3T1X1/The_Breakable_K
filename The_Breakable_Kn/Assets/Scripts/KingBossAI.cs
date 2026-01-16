using UnityEngine;
using System.Collections;

public class KingBossAI : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ustawienia walki")]
    public float lookRadius = 7f;
    public float attackRange = 2.5f; 
    public float timeBetweenComboAttacks = 0.6f; 
    public float heavyAttackCooldown = 2.5f;

    public Transform attackPoint;
    public float attackArea = 1.0f;  
    public LayerMask playerLayer;

    [Header("Granice")]
    public Transform lewaGranica;
    public Transform prawaGranica;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTarget;
    private bool isAttacking = false;
    private bool idzieWPrawo = true;
    private float nextAttackTime = 0f;
    private int comboCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void Update()
    {
        if (playerTarget == null || rb == null || isAttacking) return;

        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead) { StopMoving(); return; }

        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= lookRadius)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    StartCoroutine(BossAttackCombo());
                }
                else
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

    IEnumerator BossAttackCombo()
    {
        isAttacking = true;
        StopMoving();

        if (comboCount < 2)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.4f); 
            DealDamage(1);

            comboCount++;
            nextAttackTime = Time.time + timeBetweenComboAttacks;
        }
        else
        {
            anim.SetTrigger("HeavyAttack");
            yield return new WaitForSeconds(0.6f);
            DealDamage(2); 

            comboCount = 0;
            nextAttackTime = Time.time + heavyAttackCooldown;
        }

        isAttacking = false;
    }

    void DealDamage(int damage) 
    {
        if (attackPoint == null) return;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackArea, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage); 
            }
        }
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetInteger("AnimState", 0);
    }

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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackArea);
    }
}