// New Input System
// Double Jump

using UnityEngine.InputSystem;

[SerializeField] Rigidbody2D rb;
PlayerInputs playerInputs;
bool canDoubleJump;

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
