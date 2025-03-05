using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CameraMan : MonoBehaviour {

    public GameObject followObject;
    public float speed = 0.01f;

    [HideInInspector] public float initialX;

    void Start() {
        DontDestroyOnLoad(gameObject);
        initialX = transform.position.x;
    }

    void Update() {
        float followX = followObject.transform.position.x;
        float followY = followObject.transform.position.y;
        transform.position = new Vector3(followX, followY, -10.0f);

        //float dx = 0.0f;
        //float dy = 0.0f;

        //if (Input.GetKey(KeyCode.I))
        //{
        //    dy += speed * camera.orthographicSize;
        //}

        //if (Input.GetKey(KeyCode.K))
        //{
        //    dy -= speed * camera.orthographicSize;
        //}

        //if (Input.GetKey(KeyCode.J))
        //{
        //    dx -= speed * camera.orthographicSize;
        //}

        //if (Input.GetKey(KeyCode.L))
        //{
        //    dx += speed * camera.orthographicSize;
        //}

        //if (Input.GetKey(KeyCode.U))
        //{
        //    camera.orthographicSize -= speed * 3.0f;
        //}

        //if (Input.GetKey(KeyCode.O))
        //{
        //    camera.orthographicSize += speed * 3.0f;
        //}

        //transform.position += new Vector3(dx, dy, 0.0f);
    }
}