using UnityEngine;

abstract public class Item : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public GameObject player;
    public float floatingForce = 12.0f;
    public float floatingDistance = 0.3f;
    public float pickupRange = 2.0f;
    public float pickupForce = 30.0f;

    public int itemID = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void OwnStart()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        // Floating
        float startX = boxCollider.bounds.center.x;
        float startY = boxCollider.bounds.min.y - 0.1f;
        float distance = Physics2D.Raycast(new Vector2(startX, startY), Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground")).distance;
        if (distance < floatingDistance)
        {
            rb.AddForce(Vector2.up * floatingForce, ForceMode2D.Force);
        }


        // Picking up item
        float xDiff = player.transform.position.x - transform.position.x;
        float yDiff = player.transform.position.y - transform.position.y;
        float playerDistance = Mathf.Sqrt(Mathf.Pow(xDiff, 2.0f) + Mathf.Pow(yDiff, 2.0f));

        if (playerDistance < pickupRange)
        {
            rb.AddForce(pickupForce * new Vector2(xDiff, yDiff));
        }
    }

    abstract public string GetItemName(int itemID);
}
