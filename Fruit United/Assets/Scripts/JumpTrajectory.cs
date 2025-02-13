using UnityEngine;

public class JumpTrajectory
{
    public float a;
    public float b;
    public float c;
    public float x1;
    public float x2;

    public JumpTrajectory(float a, float b, float c, float x1, float x2)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.x1 = x1;
        this.x2 = x2;
    }

    public string toString()
    {
        return "A: " + this.a + " B: " + this.b + " C: " + this.c;
    }
}
