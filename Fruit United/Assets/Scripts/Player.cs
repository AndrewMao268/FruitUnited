using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    bool grounded;
    public float jumpHeight = 4.0f;
    public float xSpeed = 7.0f;

    public Transform feet;
    public LayerMask groundLayer;

    private float pressingHorizontal = 0.0f;
    private float pressingVertical = 0.0f;
    private float lastHorizontal = 0.0f;

    public float sizeScale = 0.2f;
    public float capsuleX = 1.5f;
    public float capsuleY = 0.5f;

    public float maxSpeedX = 10.0f;
    
    public Animator animator;

    private bool previousGround = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        grounded = false;
    }

    private void Update()
    {
        if (lastHorizontal > 0.0f)
            transform.localScale = new Vector2(-sizeScale, sizeScale);
        else
            transform.localScale = new Vector2(sizeScale, sizeScale);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // check if feet box is touching the tilemap
        // grounded = transform.Find("Feet").gameObject.GetComponent<BoxCollider2D>().IsTouching(GameObject.Find("TilemapGround").gameObject.GetComponent<TilemapCollider2D>());
        grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(capsuleX, capsuleY), CapsuleDirection2D.Horizontal, 0, groundLayer);
        

        // -0.255483, -2.785763
        // 1.827254, 0.8648027


        // Moving
        Vector2 moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        pressingHorizontal = moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Math.Abs(moveValue.x);
        pressingVertical = moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Math.Abs(moveValue.y);
        if (pressingHorizontal != 0.0f)
        {
            lastHorizontal = pressingHorizontal;
        }

        //For Animations
        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("changeSpeed", Mathf.Abs(speed));
        
        
            animator.SetBool("IsJumping", !grounded);
        
       





        // Moving Y
        if (moveValue.y == 1.0f && grounded && previousGround) {
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            return;
        }

        // Moving X
        if (Math.Abs(rb.linearVelocityX) < maxSpeedX)
            rb.AddForce(new Vector2(moveValue.x * xSpeed, 0.0f), ForceMode2D.Force);

        previousGround = grounded;
    }

    void OnCollisionEnter2D(Collision2D collision2D) {
        // A special commit from Brandon
        //if (collision2D.gameObject.name == "Soldier(Clone)" || collision2D.gameObject.name == "Soldier") {
        //    transform.position = new Vector2(0.0f, 4.0f);
        //}

        if (collision2D.gameObject.name == "Portal") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            transform.position = new Vector2(0.0f, 4.0f);
        }
    }
}





// OLD ANIMATION SCRIPT THAT FAILED. SAVED IF NEEDED
/* if (moveValue.x != 0 || moveValue.y != 0)
        {
            animator.SetFloat("X", moveValue.x);
            animator.SetFloat("Y", moveValue.y);

            animator.SetBool("IsWalking", true);
            spriteRenderer.sprite = spriteTest;
             
        } else
        {
            animator.SetBool("IsWalking", false);
            spriteRenderer.sprite = spriteTest;
        }
        */