using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLighting : MonoBehaviour
{
    public GameObject daylightCycleObj;

    private Light2D light2D;
    private DaylightCycle daylightCycle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light2D = GetComponent<Light2D>();
        daylightCycle = daylightCycleObj.GetComponent<DaylightCycle>();
    }

    // Update is called once per frame
    void Update()
    {
        light2D.color = daylightCycle.GetGradientColor;
    }
}
