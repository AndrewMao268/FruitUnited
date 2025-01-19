using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CameraMan : MonoBehaviour {

    public GameObject followObject;

    void Update() {
        float followX = followObject.transform.position.x;
        float followY = followObject.transform.position.y;
        transform.position = new Vector3(followX, followY, -10.0f);
    }
}