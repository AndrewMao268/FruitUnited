using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SpawnOfEvilAI : MonoBehaviour
{

    public Tilemap tilemap;

    public Tilemap highlightTilemap;
    public Tile highlightTile;
    public int agentHeight = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Step 1
        int z = tilemap.cellBounds.z;

        int lowestY = tilemap.cellBounds.min.y;
        int highestY = tilemap.cellBounds.max.y;

        int lowestX = tilemap.cellBounds.min.x;
        int highestX = tilemap.cellBounds.max.x;

        List<Platform> platforms = new List<Platform>();

        for (int y = lowestY; y <= highestY; y++)
        {
            //Debug.Log("Current y: " + y);
            int x = lowestX - 1;
            skipTiles(ref x, y, z);
            //Debug.Log("Initial x: " + x);

            while (x <= highestX)
            {
                int startX = x;
                measureTiles(ref x, y, z);
                int platformLength = x - startX;

                Platform platform = new Platform(new Vector3Int(startX, y, z), platformLength);
                platforms.Add(platform);
                //Debug.Log("New platform created!" + platform.toString());

                skipTiles(ref x, y, z);
            }
        }

        //Debug.Log("lowestY: " + lowestX);
        //Debug.Log("highestY: " + highestX);
        highlightPlatforms(platforms);
        printPlatforms(platforms);

        // Step 2
    }

    private bool enoughSpace(int x, int y, int z)
    {
        for (int airY = 1; airY <= agentHeight; airY++)
        {
            if (tilemap.HasTile(new Vector3Int(x, y + airY, z)))
            {
                return false;
            }
        }

        return true;
    }

    private void skipTiles(ref int x, int y, int z)
    {
        int highestX = tilemap.cellBounds.max.x;

        bool continueSkipping = true;
        while (continueSkipping)
        {
            x++;

            if (tilemap.HasTile(new Vector3Int(x, y, z)))
            {
                continueSkipping = false;
            }

            if (!enoughSpace(x, y, z))
            {
                continueSkipping = true;
            }

            if (x > highestX)
            {
                continueSkipping = false;
            }
        }
    }

    private void measureTiles(ref int x, int y, int z)
    {
        int highestX = tilemap.cellBounds.max.x;

        bool continueMeasuring = true;
        while (continueMeasuring)
        {
            x++;

            if (!tilemap.HasTile(new Vector3Int(x, y, z)))
            {
                continueMeasuring = false;
            }

            if (!enoughSpace(x, y, z))
            {
                continueMeasuring = false;
            }

            //if (tilemap.HasTile(new Vector3Int(x, y + 1, z)))
            //{
            //    continueMeasuring = false;
            //}

            if (x > highestX)
            {
                continueMeasuring = false;
            }
        }
    }

    private void highlightPlatforms(List<Platform> platforms)
    {
        highlightTilemap.ClearAllTiles();

        for (int i = 0; i < platforms.Count; i++)
        {
            Platform platform = platforms[i];
            for (int x = 0; x < platform.length; x++)
            {
                Vector3Int highlightPos = platform.start;
                highlightPos.x += x;
                highlightTilemap.SetTile(highlightPos, highlightTile);
                highlightTilemap.SetTileFlags(highlightPos, TileFlags.None);

                float shade = (float)x / platform.length;
                highlightTilemap.SetColor(highlightPos, new Color(shade, shade, shade, 1.0f));
            }
        }
    }

    private void printPlatforms(List<Platform> platforms)
    {
        string printString = "";
        for (int i = 0; i < platforms.Count; i++)
        {
            printString += platforms[i].toString() + "\n";
        }
        Debug.Log(printString);
    }
}
