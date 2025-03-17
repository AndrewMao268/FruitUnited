using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject itemsFolder;
    void Start()
    {

        System.Random rand = new System.Random();
        for (int i = 0; i < 49; i++)
        {
            float xPos = rand.Next(-500, 1270) / 10.0f;
            float yPos = 50.0f;
            GameObject item = Instantiate(itemTemplate, new Vector3(xPos, yPos), Quaternion.identity, itemsFolder.transform);
            item.GetComponent<Item>().itemID = rand.Next(0, 4);
        }
    }
}