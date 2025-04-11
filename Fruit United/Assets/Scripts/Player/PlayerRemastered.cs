using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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

    private Vector2 moveValue;

    // Jumping
    public float jumpImpulse;

    // Running
    public float maxSpeedX;
    public float accelX;

    // Attacking
    private int attackIndex;
    private System.Diagnostics.Stopwatch attackStopwatch;
    public GameObject attackCollider;
    private Vector3 attackColliderOffset;
    public int attackTimeout;
    public int attackStartTime;
    public int attackEndTime;
    [HideInInspector] public float attackDirection;
    public float attackStrength;

    private bool lastMovedToRight;

    void Start()
    {
        // Attacking
        attackIndex = 0;
        attackStopwatch = new System.Diagnostics.Stopwatch();
        attackStopwatch.Restart();
        attackColliderOffset = attackCollider.transform.position - transform.position;
    }

    void FixedUpdate()
    {
        // Grounding
        previousGround = grounded;
        grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(feetSize.x, feetSize.y), CapsuleDirection2D.Horizontal, 0, groundLayer);
        animator.SetBool("Grounded", grounded);

        
        moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Mathf.Abs(moveValue.x);
        moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Mathf.Abs(moveValue.y);
        if (Mathf.Abs(moveValue.x) > Mathf.Epsilon)
        {
            lastMovedToRight = moveValue.x > 0.0f;
        }

        // Jumping
        if (moveValue.y == 1.0f && grounded && previousGround)
        {
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
        }
        animator.SetFloat("AirSpeedY", rb.linearVelocity.y);

        // Running
        float speedX = Mathf.Abs(rb.linearVelocityX);
        if (speedX < maxSpeedX || Mathf.Sign(moveValue.x) != Mathf.Sign(rb.linearVelocityX))
        {
            rb.AddForce(new Vector2(moveValue.x * accelX, 0.0f), ForceMode2D.Force);
        }
        spriteRenderer.flipX = !lastMovedToRight;
        animator.SetInteger("AnimState", speedX > Mathf.Epsilon ? 1 : 0);

        // Attacking
        long attackTime = attackStopwatch.ElapsedMilliseconds;

        attackCollider.SetActive(attackTime > attackStartTime && attackTime < attackEndTime);
        attackCollider.transform.localPosition = new Vector3(attackColliderOffset.x * attackDirection, attackColliderOffset.y, attackColliderOffset.z);

        // Respawning
        if (transform.position.y < -20.0f)
        {
            transform.position = new Vector3(0.0f, 5.0f);
        }
    }

    void Update()
    {
        moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();

        if (Input.GetKeyDown(KeyCode.Space) && attackStopwatch.ElapsedMilliseconds > attackTimeout)
        {
            attackIndex = (attackIndex + 1) % 3;
            if (attackStopwatch.ElapsedMilliseconds > 1000)
            {
                attackIndex = 0;
            }

            animator.SetTrigger("Attack" + (attackIndex + 1));

            attackDirection = lastMovedToRight ? 1.0f : -1.0f;

            attackStopwatch.Restart();
        }
    }
}