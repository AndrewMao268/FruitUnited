using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Soldier : MonoBehaviour
{
    Rigidbody2D rb;

    // Grounding
    bool grounded;
    public Tilemap ground;

    // Getting attacked
    public GameObject attackCollider;
    public PlayerRemastered playerRemastered;

    // AI
    public GameObject player;
    
    public float jumpHeight = 4.0f;
    public float xSpeed = 7.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grounded = false;

        System.Random random = new System.Random();
        transform.position = new Vector2(random.Next(-320, 220) / 10.0f, 5.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (player.transform.position.y < -20.0f) {
            player.transform.position = new Vector2(0.0f, 4.0f);
        }

        // Soldier AI
        float moveX = player.transform.position.x - transform.position.x;
        float moveY = player.transform.position.y - transform.position.y;
        
        Vector2 moveValue = new Vector2(moveX, moveY);
        moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Math.Abs(moveValue.x);
        moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Math.Abs(moveValue.y);

        if (moveValue.y > 0.0f && grounded)
        {
            rb.AddForce(new Vector2(0.0f, jumpHeight), ForceMode2D.Impulse);
            return;
        }

        rb.AddForce(new Vector2(moveValue.x * xSpeed, 0.0f), ForceMode2D.Force);

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0.0f);

        if (transform.position.y < -20.0f)
        {
            transform.position = new Vector3(0.0f, 20.0f, 0.0f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (ReferenceEquals(collision2D.gameObject, ground))
        {
            grounded = true;
        }

        
    }

    void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (ReferenceEquals(collision2D.gameObject, attackCollider))
        {
            rb.AddForce(new Vector2(playerRemastered.attackDirection * playerRemastered.attackStrength, 0.0f), ForceMode2D.Impulse);
        }
    }

    void OnCollisionExit2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.name == "TilemapGround")
        {
            grounded = false;
        }
    }
}
