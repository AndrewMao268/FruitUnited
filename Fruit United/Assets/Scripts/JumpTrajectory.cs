using UnityEngine;

public class JumpTrajectory : Trajectory
{
    public float a;
    public float b;
    public float c;

    public float x1;
    public float y1;
    public float x2;
    public float y2;

    Platform startPlatform;
    Platform endPlatform;

    public float idealSpeed;
    public int id;

    public JumpTrajectory(float a, float b, float c, float x1, float y1, float x2, float y2, Platform startPlatform, Platform endPlatform, float idealSpeed)
    {
        this.a = a;
        this.b = b;
        this.c = c;

        this.x1 = x1; this.y1 = y1;
        this.x2 = x2; this.y2 = y2;

        this.startPlatform = startPlatform;
        this.endPlatform = endPlatform;

        this.id = (int)((int)a * 23974987 + (int)b * 983475954352 + (int)c * 293748923423427);
        this.idealSpeed = idealSpeed;
    }

    public float plugIn(float x)
    {
        return a * Mathf.Pow(x, 2.0f) + b * x + c;
    }

    public string toString()
    {
        return "A: " + this.a + " B: " + this.b + " C: " + this.c + " X1: " + this.x1 + " Y1: " + this.y1 + " X2: " + this.x2 + " Y2: " + this.y2 + " ID: " + this.id;
    }
}
