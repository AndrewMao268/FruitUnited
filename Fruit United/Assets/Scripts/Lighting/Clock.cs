using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public GameObject daylightCycleObj;

    private TMP_Text text;
    private DaylightCycle daylightCycle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        daylightCycle = daylightCycleObj.GetComponent<DaylightCycle>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = daylightCycle.GetTimeString;
    }
}
