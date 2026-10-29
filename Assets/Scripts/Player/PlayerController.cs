using UnityEngine;
using UnityEngine.InputSystem;

// Movimento do jogador: andar, pular e detectar o chão. Só comanda parâmetros do Animator.
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");

    [SerializeField] private ClassData classData;

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveSpeed;
    private float inputX;
    private bool jumpRequested;

    public ClassData ClassData => classData;
    public bool IsGrounded { get; private set; }
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveSpeed = classData.moveSpeed;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJumpPressed;
        jumpAction.action.canceled += OnJumpReleased;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpReleased;
    }

    private void Update()
    {
        inputX = CanMove ? moveAction.action.ReadValue<Vector2>().x : 0f;
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (inputX > 0.01f) Face(1f);
        else if (inputX < -0.01f) Face(-1f);

        animator.SetFloat(SpeedHash, Mathf.Abs(inputX));
        animator.SetBool(IsGroundedHash, IsGrounded);
        animator.SetFloat(VelocityYHash, rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = inputX * moveSpeed;

        if (jumpRequested && IsGrounded)
        {
            velocity.y = jumpForce;
            AudioManager.Play(AudioManager.Sfx.Jump);
        }
        jumpRequested = false;

        rb.linearVelocity = velocity;
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (CanMove && Time.timeScale > 0f) jumpRequested = true;
    }

    // Soltar o botão cedo corta o pulo (pulo de altura variável).
    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
    }

    // Vira o personagem pela escala, para que colliders e pontos de ataque virem junto.
    private void Face(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
