using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;


public class Player : MonoBehaviour
{

    Rigidbody2D rb;
    
    public float jumpHeight = 4.0f;
    public float xSpeed = 7.0f;

    public Transform feet;
    public LayerMask groundLayer;

    private float pressingHorizontal = 0.0f;
    private float pressingVertical = 0.0f;
    private float lastHorizontal = 0.0f;

    public float xAccel = 7.0f;
    public float maxSpeedX = 10.0f;

    public CapsuleCollider2D capsuleCollider2D;

    public float capsuleX = 1.5f;
    public float capsuleY = 0.5f;

    private bool grounded;
    private bool previousGround = false;
    public float jumpImpulse = 4.0f;

    private GameObject targetObject;

    private System.Diagnostics.Stopwatch stopwatch;
    private string recordX;
    private string recordY;

    public string newFirstLine = "You found it!";

    public Npcbrandontestscript npcbrandontestscript;

    private List<int> inventoryStorage;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        grounded = false;
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Restart();

        recordX = "";
        recordY = "";

        inventoryStorage = new List<int>();
        for (int i = 0; i < 27; i++)
        {
            inventoryStorage.Add(0);
        }
    }

    private void Update()
    {
        if (lastHorizontal > 0.0f)
        {
            //attack "sword" collider
            capsuleCollider2D.offset = new Vector2(-0.5f, capsuleCollider2D.offset.y);
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }
        else
        {
            capsuleCollider2D.offset = new Vector2(-0.4f, capsuleCollider2D.offset.y);
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if feet box is touching the tilemap
        grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(capsuleX, capsuleY), CapsuleDirection2D.Horizontal, 0, groundLayer);

        // -0.255483, -2.785763
        // 1.827254, 0.8648027


        // Moving
        Vector2 moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        pressingHorizontal = moveValue.x = moveValue.x == 0.0f ? 0.0f : moveValue.x / Mathf.Abs(moveValue.x);
        pressingVertical = moveValue.y = moveValue.y == 0.0f ? 0.0f : moveValue.y / Mathf.Abs(moveValue.y);
        if (pressingHorizontal != 0.0f)
        {
            lastHorizontal = pressingHorizontal;
        }

        // Moving Y
        if (moveValue.y == 1.0f && grounded && previousGround)
        {
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            stopwatch.Restart();
            recordX = "";
            recordY = "";
            return;
        }

        if (!grounded)
        {
            recordX += stopwatch.Elapsed.TotalSeconds + ", ";
            recordY += transform.position.y + ", ";
        }

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //    DateTime currentTime = DateTime.UtcNow;
        //    long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        //    using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "MysteriousText" + unixTime + ".txt"), false))
        //    {
        //        outputFile.WriteLine(recordX);
        //        outputFile.WriteLine(recordY);
        //    }
        //}

        // Moving X
        if (Mathf.Abs(rb.linearVelocityX) < maxSpeedX)
        {
            rb.AddForce(new Vector2(moveValue.x * xAccel, 0.0f), ForceMode2D.Force);
        }

        previousGround = grounded;


        //early in development attacking
        //if (targetObject != null && Input.GetKeyDown(KeyCode.F))
        //{
        //    Destroy(targetObject);
        //    targetObject = null;
        //}
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.name == "Portal")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            
            transform.position = new Vector2(0.0f, 4.0f);

            GameObject gridObject = GameObject.Find("Grid"); // Find the Grid GameObject
            Transform targetTilemap = gridObject.transform.Find("LeftBarrier");
            TilemapRenderer tilemapRenderer = targetTilemap.GetComponent<TilemapRenderer>();
            Rigidbody2D rigidbody2D = targetTilemap.GetComponent<Rigidbody2D>();
            CompositeCollider2D compositeCollider2D = targetTilemap.GetComponent<CompositeCollider2D>();
            TilemapCollider2D tilemapCollider2D = targetTilemap.GetComponent<TilemapCollider2D>();


            npcbrandontestscript.ChangeFirstDialogue(newFirstLine);

            if (tilemapRenderer != null)
            {
                tilemapCollider2D.enabled = false;
                tilemapRenderer.enabled = false;
            }

            if (rigidbody2D != null)
            {
                Destroy(compositeCollider2D);
                Destroy(rigidbody2D); 

            }
        }

        if (collision2D.gameObject.name == "Sword" || collision2D.gameObject.name == "Sword(Clone)")
        {
            Destroy(collision2D.gameObject);
            Item itemScript = collision2D.gameObject.GetComponent<Item>();
            int itemID = itemScript.itemID;
            inventoryStorage[itemID]++;

            string inventoryString = "";
            inventoryString += "I picked up a " + itemScript.GetItemName(itemID) + "!\n";
            for (int i = 0; i < 4; i++)
            {
                inventoryString += "I have " + inventoryStorage[i] + " " + itemScript.GetItemName(i) + "s.\n";
            }
            Debug.Log(inventoryString);
        }
 
    }

    




    //early in development attacking
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Soldiers"))
        {
            targetObject = col.gameObject;
        }
    }

    public bool IsGrounded
    {
        get => grounded;
    }
    public float XSpeed
    {
        get => Mathf.Abs(rb.linearVelocity.x);
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