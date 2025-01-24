using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Display_Text : MonoBehaviour
{
    [SerializeField] Text obj_text;
    [SerializeField] InputField display;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obj_text.text = PlayerPrefs.GetString("user_name");
    }

    public void Create()
    {
        obj_text.text = display.text;
        PlayerPrefs.SetString("user_name", obj_text.text);
        PlayerPrefs.Save();
    }
}
