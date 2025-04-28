using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CameraMan : MonoBehaviour
{
    public Camera camera;

    public GameObject followObject;
    public float translationSpeed = 1.0f;
    public float translationThreshold = 0.01f;
    public float heightSpeed = 1.0f;
    public float heightThreshold = 0.01f;
    public float minOrthographicSize = 5.0f;
    public float yOffset;

    public GameObject player;

    // Soldiers
    public GameObject soldiersFolder;
    public GameObject soldier;
    public List<GameObject> soldiers;

    [HideInInspector] public float initialX;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

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
        float followX = followObject.transform.position.x;
        float followY = followObject.transform.position.y + yOffset;
        transform.position = new Vector3(followX, followY, -10.0f);

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

        float width = Mathf.Max(maxX - minX, 2 * Mathf.Abs(maxX - followX), 2 * Mathf.Abs(followX - minX));
        float height = Mathf.Max(maxY - minY, 2 * Mathf.Abs(maxY - followY), 2 * Mathf.Abs(followY - minY));
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

        // camera.orthographicSize = Mathf.Max(orthographicSize, minOrthographicSize);
        // camera.orthographicSize = 10.0f;

        float targetOrthoSize = Mathf.Max(orthographicSize, minOrthographicSize);

        float deltaOrtho = targetOrthoSize - camera.orthographicSize;
        float velocityOrtho = Mathf.Sign(deltaOrtho) * Mathf.Abs(deltaOrtho) * heightSpeed;

        if (Mathf.Abs(velocityOrtho) < heightThreshold)
        {
            velocityOrtho = 0.0f;
        }

        camera.orthographicSize += velocityOrtho;

        float targetX = (minX + maxX) * 0.5f;
        float targetY = (minY + maxY) * 0.5f;

        float deltaX = targetX - transform.position.x;
        float deltaY = targetY - transform.position.y;

        float angle = Mathf.Atan2(deltaY, deltaX);

        velocityX = Mathf.Cos(angle) * Mathf.Abs(deltaX) * translationSpeed;
        velocityY = Mathf.Sin(angle) * Mathf.Abs(deltaY) * translationSpeed;

        if (Mathf.Abs(velocityX) < translationThreshold)
        {
            velocityX = 0.0f;
        }
        if (Mathf.Abs(velocityY) < translationThreshold)
        {
            velocityY = 0.0f;
        }

        float newPosX = transform.position.x + velocityX;
        float newPosY = transform.position.y + velocityY;

        // transform.position = new Vector3(newPosX, newPosY, -10.0f);

        //Debug.Log("velocityX: " + velocityX);
        //Debug.Log("velocityY: " + velocityY);
        //Debug.Log("velocityOrtho: " + velocityOrtho);
    }
}