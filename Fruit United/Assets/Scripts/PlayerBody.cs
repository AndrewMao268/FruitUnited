using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour {

    Player playerBehaviour;

    public Animator animator;

    private void Start()
    {
        playerBehaviour = GameObject.Find("Player").GetComponent<MonoBehaviour>() as Player;
    }
    private void FixedUpdate()
    {
        animator.SetBool("IsJumping", !playerBehaviour.IsGrounded);
        animator.SetFloat("changeSpeed", playerBehaviour.XSpeed);
    }
}