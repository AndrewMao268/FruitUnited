using System;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{

    public Gradient globalLightGradient;

    public int millisecondsPerDay = 1000 * 60 * 1;

    private System.Diagnostics.Stopwatch stopwatch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Restart();
    }

    // Update is called once per frame
    void Update()
    {
        int minutesSinceMidnight = (int)(GetTimeProportion * 60 * 24);
        TimeSpan timeSpan = TimeSpan.FromMinutes(minutesSinceMidnight);
        Debug.Log(timeSpan.ToString(@"hh\:mm"));
    }

    public float GetTimeProportion
    {
        get {
            double milliseconds = stopwatch.Elapsed.TotalMilliseconds;

            float timeProportion = (float)((milliseconds % millisecondsPerDay) / millisecondsPerDay);

            return timeProportion;
        }
    }

    public Color GetGradientColor
    {
        get
        {
            return globalLightGradient.Evaluate(GetTimeProportion);
        }
    }
}
