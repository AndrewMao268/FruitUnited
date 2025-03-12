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

    private void Start()
    {
        canDestroy = new List<string>();
        canDestroy.Add("Soldier");
        canDestroy.Add("Soldier(Clone)");

        myCollider2D = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        Debug.Log("HIT " + collision2D.gameObject.name);
        if (canDestroy.Contains(collision2D.gameObject.name))
        {
            Debug.Log("DESTROYED " + collision2D.gameObject.name);
            Destroy(collision2D.gameObject);
        }
    }
}