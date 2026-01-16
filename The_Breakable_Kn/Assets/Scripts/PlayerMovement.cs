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
        if (!this.enabled) return;

        if (GetComponent<PlayerHealth>() != null && GetComponent<PlayerHealth>().isDead) return;

        if (context.performed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

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

    private void Attack()
    {
        if (attackPoint == null)
        {
            return;
        }

        anim.SetTrigger("Attack1");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}