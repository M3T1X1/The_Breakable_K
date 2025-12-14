using UnityEngine;
using UnityEngine.InputSystem; // <<< DODAJ T� LINI�

public class PlayerMovement : MonoBehaviour
{
    // Ustawienia, kt�re mo�esz zmienia� w Unity Inspectorze
    public float runSpeed = 5f;
    public float jumpForce = 8f;

    // Prywatne zmienne techniczne
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    // Zmienne do przechowywania aktualnego stanu wej�cia
    private Vector2 moveInput; // Przechowuje kierunek ruchu (X i Y)
    private bool jumpPressed;   // Czy gracz nacisn�� skok?

    [Header("Wykrywanie Ziemi")]
    public LayerMask groundLayer;
    private bool isGrounded;

    // Start jest wywo�ywany raz, gdy obiekt si� pojawia
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update jest teraz u�ywany TYLKO do fizyki (ruch i skok)
    void Update()
    {
        HandleHorizontalMovement();
        HandleJumping();
    }

    // FUNKCJA ODCZYTU KIERUNKU (WYZWALANA PRZEZ NOWY SYSTEM WEJ�CIA)
    public void OnMove(InputAction.CallbackContext context)
    {
        // Odczytuje, czy klawisze A/D s� wci�ni�te
        moveInput = context.ReadValue<Vector2>();
    }

    // FUNKCJA ODCZYTU SKOKU (WYZWALANA PRZEZ NOWY SYSTEM WEJ�CIA)
    public void OnJump(InputAction.CallbackContext context)
    {
        // Sprawdza, czy przycisk zosta� naci�ni�ty
        if (context.performed)
        {
            jumpPressed = true;
        }
    }


    private void HandleHorizontalMovement()
    {
        // U�ywa moveInput, kt�re jest aktualizowane przez OnMove
        rb.linearVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);

        // Sprawdzenie, czy się poruszamy (moveInput.x != 0)
        bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;

        // Ustawienie parametru "Run" w Animatorze (który jest już w kontrolerze)
        anim.SetBool("Run", isMoving);
    }

    private void HandleJumping()
    {
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);

        // Używamy parametru "Grounded", który jest w Twoim gotowym Animatorze
        anim.SetBool("Grounded", isGrounded);

        // Sprawdza, czy klawisz skoku jest wci�ni�ty i czy dotyka ziemi
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Używamy parametru "Jump", który jest w Twoim gotowym Animatorze
            anim.SetTrigger("Jump");
        }

        // Resetuje stan klawisza skoku, �eby nie skaka� w niesko�czono��
        jumpPressed = false;
    }

    // FUNKCJA ODCZYTU ATAKU (WYZWALANA PRZEZ NOWY SYSTEM WEJŚCIA)
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Sprawdza, czy przycisk został naciśnięty
        if (context.performed)
        {
            // Wyzwalanie animacji ataku (użyjemy Attack1, która jest w Animatorze)
            anim.SetTrigger("Attack1");
        }
    }
}