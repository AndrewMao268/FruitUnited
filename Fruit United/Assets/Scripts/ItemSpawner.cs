using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemTemplate;
    void Start()
    {

        System.Random rand = new System.Random();
        for (int i = 0; i < 4; i++)
        {
            float xPos = rand.Next(-500, 1270) / 10.0f;
            float yPos = 50.0f;
            GameObject item = Instantiate(itemTemplate, new Vector3(xPos, yPos), Quaternion.identity);
            item.GetComponent<Item>().itemID = rand.Next(0, 4);
        }
    }
}