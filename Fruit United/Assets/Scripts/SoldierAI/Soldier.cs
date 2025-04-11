using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Soldier : MonoBehaviour
{
    Rigidbody2D rb;
    System.Random random;

    // Grounding
    public Transform feetTransform;
    public Vector2 feetSize;
    public LayerMask groundMask;
    bool grounded;
    public Tilemap ground;

    // Attacking
    public float attackStrength;
    [HideInInspector] public float attackDirection;
    public GameObject playerHitCollider;

    // Getting attacked
    public GameObject attackCollider;
    public PlayerRemastered playerRemastered;

    // AI
    public GameObject player;
    
    public float jumpHeight;
    public float maxSpeedX;
    public float accelX;

    private bool lastMovedToRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grounded = false;

        random = new System.Random();
        transform.position = new Vector2(random.Next(-320, 220) / 10.0f, 5.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCapsule(feetTransform.position, new Vector2(feetSize.x, feetSize.y), CapsuleDirection2D.Horizontal, 0.0f, groundMask);

        // Soldier AI
        float moveX = player.transform.position.x - transform.position.x;
        float moveY = player.transform.position.y - transform.position.y;
        
        Vector2 moveValue = new Vector2(moveX, moveY);
        moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Math.Abs(moveValue.x);
        moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Math.Abs(moveValue.y);
        if (Mathf.Abs(moveValue.x) > Mathf.Epsilon)
        {
            lastMovedToRight = moveValue.x > 0.0f;
        }

        // Jumping
        if (moveValue.y > 0.0f && grounded)
        {
            rb.AddForce(new Vector2(0.0f, jumpHeight), ForceMode2D.Impulse);
        }

        // Running
        float speedX = Mathf.Abs(rb.linearVelocityX);
        if (speedX < maxSpeedX || Mathf.Sign(moveValue.x) != Mathf.Sign(rb.linearVelocityX))
        {
            rb.AddForce(new Vector2(moveValue.x * accelX, 0.0f), ForceMode2D.Force);
        }

        // Attacking
        attackDirection = lastMovedToRight ? 1.0f : -1.0f;

        // Respawning
        if (transform.position.y < -20.0f)
        {
            transform.position = new Vector2(0.0f, 5.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (ReferenceEquals(collision2D.gameObject, attackCollider))
        {
            float impulseX = playerRemastered.attackDirection * playerRemastered.attackStrength;
            float impulseY = (float)random.NextDouble() * playerRemastered.attackStrength;
            rb.AddForce(new Vector2(impulseX, impulseY), ForceMode2D.Impulse);
        }

        if (ReferenceEquals(collision2D.gameObject, playerHitCollider))
        {
            float impulseX = attackDirection * attackStrength;
            float impulseY = (float)random.NextDouble() * attackStrength;
            playerRemastered.rb.AddForce(new Vector2(impulseX, impulseY), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        
    }
}
