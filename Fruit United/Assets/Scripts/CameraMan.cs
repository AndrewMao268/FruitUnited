using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CameraMan : MonoBehaviour
{
    public Camera camera;

    public GameObject followObject;
    public float speed = 0.01f;
    public float yOffset;

    public GameObject player;

    // Soldiers
    public GameObject soldiersFolder;
    public GameObject soldier;
    public List<GameObject> soldiers;

    [HideInInspector] public float initialX;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        initialX = transform.position.x;

        soldiers = new List<GameObject>();
        soldiers.Add(soldier);

        for (int i = 0; i < 9; i++)
        {
            GameObject newSoldier = Instantiate(soldier, soldiersFolder.transform);
            soldiers.Add(newSoldier);
        }
    }

    void Update()
    {
        //float followX = followObject.transform.position.x;
        //float followY = followObject.transform.position.y + yOffset;
        //transform.position = new Vector3(followX, followY, -10.0f);

        float minX = player.transform.position.x;
        float maxX = player.transform.position.x;
        float minY = player.transform.position.y;
        float maxY = player.transform.position.y;

        GameObject minXObj = player;
        GameObject maxXObj = player;
        GameObject minYObj = player;
        GameObject maxYObj = player;

        for (int i = 0; i < soldiers.Count; i++)
        {
            GameObject obj = soldiers[i];
            Vector3 pos = soldiers[i].transform.position;
            if (pos.x < minX)
            {
                minX = pos.x;
                minXObj = obj;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
                maxXObj = obj;
            }
            if (pos.y < minY)
            {
                minY = pos.y;
                minYObj = obj;
            }
            if (pos.y > maxY)
            {
                maxY = pos.y;
                maxYObj = obj;
            }
        }

        minX = minXObj.GetComponent<SpriteRenderer>().bounds.min.x;
        maxX = maxXObj.GetComponent<SpriteRenderer>().bounds.max.x;
        minY = minYObj.GetComponent<SpriteRenderer>().bounds.min.y;
        maxY = maxYObj.GetComponent<SpriteRenderer>().bounds.max.y;

        float width = maxX - minX;
        float height = maxY - minY;
        float neededAspect = width / height;
        float actualAspect = Screen.width / Screen.height;

        float orthographicSize = height * 0.5f;
        if (neededAspect < actualAspect)
        {
            orthographicSize = height * 0.5f;
        }
        else
        {
            orthographicSize = width * (1.0f / actualAspect) * 0.5f;
        }

        camera.orthographicSize = orthographicSize;
        // camera.orthographicSize = 10.0f;

        float posX = (minX + maxX) * 0.5f;
        float posY = (minY + maxY) * 0.5f;
        transform.position = new Vector3(posX, posY, -10.0f);
    }
}