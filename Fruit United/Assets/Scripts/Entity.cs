using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    public float xAccel = 7.0f;
    public float maxSpeedX = 10.0f;

    public Transform feet;
    public LayerMask groundLayer;
    public float capsuleX = 1.5f;
    public float capsuleY = 0.5f;

    public float xSpeed = 7.0f;

    protected bool grounded;
    protected bool previousGround = false;
    public float jumpImpulse = 4.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grounded = false;

        OwnStart();
    }

    protected abstract void OwnStart();

    // Update is called once per frame
    void FixedUpdate()
    {
        //grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(capsuleX, capsuleY), CapsuleDirection2D.Horizontal, 0, groundLayer);

        //Vector2 moveValue = GetInput();

        //// Moving Y
        //if (moveValue.y == 1.0f && grounded && previousGround)
        //{
        //    rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
        //}

        //// Moving X
        //if (Math.Abs(rb.linearVelocityX) < maxSpeedX)
        //{
        //    rb.AddForce(new Vector2(moveValue.x * xAccel, 0.0f), ForceMode2D.Force);
        //}

        //previousGround = grounded;

        OwnFixedUpdate();
    }

    protected abstract void OwnFixedUpdate();
}
