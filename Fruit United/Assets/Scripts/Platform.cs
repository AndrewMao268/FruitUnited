using UnityEngine;

public class Platform
{
    public Vector3Int start;
    public int length;

    public Platform(Vector3Int start, int length)
    {
        this.start = start;
        this.length = length;
    }

    public string toString()
    {
        return "Start: " + this.start + " Length: " + this.length;
    }
}