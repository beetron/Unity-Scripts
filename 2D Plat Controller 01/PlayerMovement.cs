using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpPower = 16f;
    
    float horizontal;
    PlayerInputs playerInputs;
    bool isFacingRight = true;
    bool canDoubleJump;

    void Awake()
    {
        playerInputs = new PlayerInputs();
    }

    void OnEnable()
    {
        playerInputs.Enable();
    }

    void OnDisable()
    {
        playerInputs.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // Flip character sprite when walking opposite direction
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Jump
    public void Jump(InputAction.CallbackContext context)
    {
        // Jump only if grounded
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            canDoubleJump = true;
        }

        // Shorter jump height if jump is tapped
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Double jump
        if (!IsGrounded() && context.performed && canDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            canDoubleJump = false;
        }
    }

    // Move
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }
}
