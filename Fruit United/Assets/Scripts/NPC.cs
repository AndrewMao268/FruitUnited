using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject player;
    public GameObject interactionText;

    public float interactionDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool closeEnough = GetDistanceFromPlayer() < interactionDistance;
        interactionText.SetActive(closeEnough);
        if (closeEnough && Input.GetKeyDown(KeyCode.F))
        {
            ShowDialogue();
        }
    }

    private void Checks()
    {
        if (interactionDistance < 0.0f)
        {
            throw new System.Exception("interactionDistance cannot be negative");
        }
    }

    private float GetDistanceFromPlayer()
    {
        float xDistance = player.transform.position.x - transform.position.x;
        float yDistance = player.transform.position.y - transform.position.y;
        return Mathf.Sqrt(Mathf.Pow(xDistance, 2.0f) + Mathf.Pow(yDistance, 2.0f));
    }

    private void ShowDialogue()
    {
        Debug.Log("Dialogue!");
    }
}
