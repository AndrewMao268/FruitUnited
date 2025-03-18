using UnityEngine;

public class RunTrajectory : Trajectory
{
    public float x1;
    public float y1;
    public float x2;
    public float y2;

    public RunTrajectory(float x1, float y1, float x2, float y2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }
   

    public string toString()
    {
        return "X1: " + this.x1 + " Y1: " + this.y1 + " X2: " + this.x2 + " Y2: " + this.y2;
    }
}