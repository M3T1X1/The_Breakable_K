using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch i Fizyka")]
    public float runSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Walka")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private PlayerHealth playerHealth;
    // --- KLUCZOWA POPRAWKA: Deklaracja zmiennej ---
    private SpriteRenderer spriteRenderer;

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
        playerHealth = GetComponent<PlayerHealth>();
        // --- KLUCZOWA POPRAWKA: Pobranie komponentu ---
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Automatyczne wykrycie kierunku na starcie na podstawie skali
        facingRight = (transform.localScale.x > 0);

        if (GameManager.instance != null && GameManager.instance.useSpawnPos)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            transform.position = GameManager.instance.playerSpawnPos;
            rb.WakeUp(); // Budzimy fizykê po teleportacji
            anim.Rebind(); // Resetujemy animatora
            GameManager.instance.useSpawnPos = false;
        }
    }

    void Update()
    {
        // Rêczny odczyt klawiatury - najbardziej niezawodny po zmianie sceny
        float horizontal = 0;
        if (Keyboard.current.dKey.isPressed) horizontal = 1;
        else if (Keyboard.current.aKey.isPressed) horizontal = -1;

        moveInput = new Vector2(horizontal, 0);

        HandleHorizontalMovement();
        HandleJumping();
    }

    // Pozostawiamy dla kompatybilnoœci z Input System, ale Update robi g³ówn¹ robotê
    public void OnMove(InputAction.CallbackContext context) { }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) jumpPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!this.enabled) return;
        if (playerHealth != null && playerHealth.isDead) return;

        if (context.performed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void HandleHorizontalMovement()
    {
        if (playerHealth != null && playerHealth.isDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;
        anim.SetInteger("AnimState", isMoving ? 1 : 0);

        // Wywo³anie obracania
        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;

        // Obracamy obrazek
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }

        // NAPRAWA ATAKU: Przesuwamy punkt ataku na drug¹ stronê
        if (attackPoint != null)
        {
            // Odwracamy lokaln¹ pozycjê X punktu ataku
            Vector3 newPos = attackPoint.localPosition;
            newPos.x *= -1;
            attackPoint.localPosition = newPos;
        }
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

    private void Attack()
    {
        if (attackPoint == null) return;
        anim.SetTrigger("Attack1");

        int currentDamage = attackDamage;
        if (PlayerHealth.swordCharges > 0) currentDamage = playerHealth.boostedDamage;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hitEnemies.Length > 0 && PlayerHealth.swordCharges > 0)
        {
            PlayerHealth.swordCharges--;
            playerHealth.UpdateUI();
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(currentDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}