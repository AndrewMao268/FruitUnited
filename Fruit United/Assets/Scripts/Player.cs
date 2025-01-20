using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    bool grounded;
    public float jumpHeight = 4.0f;
    public float xSpeed = 7.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        grounded = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Math.Abs(moveValue.x);
        moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Math.Abs(moveValue.y);

        if (moveValue.y == 1.0f && grounded) {
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            return;
        }

        rb.AddForce(new Vector2(moveValue.x * xSpeed, 0.0f), ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D collision2D) {
        if (collision2D.gameObject.name == "TilemapGround")
        {
            grounded = true;
        }

        if (collision2D.gameObject.name == "Soldier(Clone)") {
            transform.position = new Vector2(0.0f, 4.0f);
        }

        if (collision2D.gameObject.name == "Portal") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            transform.position = new Vector2(0.0f, 4.0f);
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
