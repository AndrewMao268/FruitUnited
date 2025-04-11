using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRemastered : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // Grounding
    [HideInInspector] public bool grounded;
    private bool previousGround;
    public Transform feet;
    public Vector2 feetSize;
    public LayerMask groundLayer;

    // Jumping
    public float jumpImpulse;


    // Running
    public float maxSpeedX;
    public float accelX;

    // Attacking
    private int attackIndex;
    private System.Diagnostics.Stopwatch attackStopwatch;

    private bool pressingHorizontal;
    private bool pressingVertical;
    private bool facingLeft = false;

    void Start()
    {
        // Attacking
        attackIndex = 0;
        attackStopwatch = new System.Diagnostics.Stopwatch();
        attackStopwatch.Restart();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Grounding
        previousGround = grounded;
        grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(feetSize.x, feetSize.y), CapsuleDirection2D.Horizontal, 0, groundLayer);
        animator.SetBool("Grounded", grounded);

        Vector2 moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Mathf.Abs(moveValue.x);
        moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Mathf.Abs(moveValue.y);
        pressingHorizontal = moveValue.x > 0.0f;
        pressingVertical = moveValue.y > 0.0f;

        // Jumping
        if (moveValue.y == 1.0f && grounded && previousGround)
        {
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
            return;
        }
        animator.SetFloat("AirSpeedY", rb.linearVelocity.y);

        // Running
        float speedX = Mathf.Abs(rb.linearVelocityX);
        if (speedX < maxSpeedX)
        {
            rb.AddForce(new Vector2(moveValue.x * accelX, 0.0f), ForceMode2D.Force);
        }
        spriteRenderer.flipX = moveValue.x < 0.0f;
        animator.SetInteger("AnimState", speedX > Mathf.Epsilon ? 1 : 0);

        //ChatGPT told me to fix movement (next section) so the feet are not moving when it looks like the character is going nowwhere - Brandon
        if (speedX > 2.0f)
        {
            animator.SetInteger("AnimState",1 ); // Walking state
        }
        else
        {
            animator.SetInteger("AnimState", 0); // Idle state
            if (moveValue.x == 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y); // or even 0 to stop instantly
            }
        }

        // Player facing direction fix
        float inputDirection = Input.GetAxisRaw("Horizontal");
        if (inputDirection < 0)
        {
            facingLeft = true;
        }
        else if (inputDirection > 0)
        {
            facingLeft = false;
        }
        spriteRenderer.flipX = facingLeft;

        // Attacking
        if (Input.GetKeyDown(KeyCode.Space) && attackStopwatch.ElapsedMilliseconds > 250)
        {
            attackIndex = (attackIndex + 1) % 3;
            if (attackStopwatch.ElapsedMilliseconds > 1000)
            {
                attackIndex = 0;
            }

            animator.SetTrigger("Attack" + (attackIndex + 1));

            attackStopwatch.Restart();
        }
    }
}