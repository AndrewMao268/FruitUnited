using UnityEngine;

public class Platform
{
    public Vector3Int start;
    public int length;

    public int x1;
    public int y1;
    public int x2;
    public int y2;

    public Platform(Vector3Int start, int length)
    {
        this.start = start;
        this.length = length;

        this.x1 = start.x;
        this.y1 = start.y;
        this.x2 = start.x + length - 1;
        this.y2 = start.y;
    }

    public string toString()
    {
        return "Start: " + this.start + " Length: " + this.length;
    }
}