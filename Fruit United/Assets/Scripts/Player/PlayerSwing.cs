using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;


public class PlayerSwing : MonoBehaviour
{

    private List<string> canDestroy;
    private Collider2D myCollider2D;

    [HideInInspector] public float lastHorizontal = 0.0f;
    public float force = 10.0f;

    private void Start()
    {
        canDestroy = new List<string>();
        canDestroy.Add("Soldier");
        canDestroy.Add("Soldier(Clone)");

        myCollider2D = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (canDestroy.Contains(collision2D.gameObject.name))
        {
            collision2D.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(lastHorizontal * force, force * 0.2f), ForceMode2D.Impulse);
        }
    }
}