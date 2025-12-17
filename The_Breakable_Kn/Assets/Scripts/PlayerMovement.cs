using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // --- USTAWIENIA RUCHU ---
    [Header("Ruch i Fizyka")]
    public float runSpeed = 5f;
    public float jumpForce = 8f;

    // --- SYSTEM WALKI (NOWE ZMIENNE) ---
    [Header("Walka")]
    public Transform attackPoint;      // Przeciągnij tu obiekt AttackPoint
    public float attackRange = 0.5f;   // Zasięg uderzenia (widoczny jako czerwona kula)
    public LayerMask enemyLayers;      // Wybierz warstwę "Enemy"
    public int attackDamage = 20;      // Ile HP zabierasz magowi
    public float attackRate = 2f; // Ile ataków na sekundę (np. 2)
    private float nextAttackTime = 0f; // Kiedy będzie można zaatakować kolejny raz

    // --- KOMPONENTY ---
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    // --- ZMIENNE LOGICZNE ---
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool facingRight = true;

    [Header("Wykrywanie Ziemi")]
    public LayerMask groundLayer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleHorizontalMovement();
        HandleJumping();
    }

    // --- SYSTEM WEJŚCIA (INPUT SYSTEM) ---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) jumpPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GetComponent<PlayerHealth>() != null && GetComponent<PlayerHealth>().isDead) return;

        if (context.performed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // --- LOGIKA RUCHU ---
    private void HandleHorizontalMovement()
    {
        if (GetComponent<PlayerHealth>() != null && GetComponent<PlayerHealth>().isDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;
        anim.SetInteger("AnimState", isMoving ? 1 : 0);

        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void HandleJumping()
    {
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("AirSpeedY", rb.linearVelocity.y);

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
        jumpPressed = false;
    }

    // --- LOGIKA WALKI (GŁÓWNA FUNKCJA) ---
    private void Attack()
    {
        // 1. Odpal animację
        anim.SetTrigger("Attack1");

        // 2. Sprawdź czy w obiekcie attackPoint są wrogowie
        // Tworzymy niewidzialne koło o zasięgu attackRange
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Zadaj obrażenia każdemu trafionemu wrogowi
        foreach (Collider2D enemy in hitEnemies)
        {
            // Szukamy skryptu EnemyHealth na Magu
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Rysuje czerwoną kulę w edytorze, żeby łatwiej było ustawić zasięg
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}