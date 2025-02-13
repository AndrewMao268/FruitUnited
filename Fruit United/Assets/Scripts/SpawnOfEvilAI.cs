using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using QuikGraph;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.Edge<Platform>>;

public class SpawnOfEvilAI : MonoBehaviour
{

    public Tilemap tilemap;

    public Tilemap highlightTilemap;
    public Tile highlightTile;
    public GameObject trajectoryBrush;
    public int agentHeight = 1;

    public float jumpHeight = 5.0f;

    private List<JumpTrajectory> trajectories;
    public int trajectoryView = 0;

    private List<GameObject> brushes = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trajectories = new List<JumpTrajectory>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject go in brushes)
        {
            Destroy(go);
        }
        trajectories.Clear();


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

        highlightATile(new Vector3Int(0, 0, 0), new Color(0.5f, 0.0f, 1.0f, 1.0f));

        // Step 2

        SurfaceGraph graph = new SurfaceGraph();

        for (int i = 0; i < platforms.Count; i++)
        {
            for (int j = i + 1; j < platforms.Count; j++)
            {
                handleEdgeCreation(platforms[i], platforms[j], ref graph);

                Platform lowPlatform;
                Platform highPlatform;

                if (platforms[i].start.y < platforms[j].start.y)
                {

                }


                float startX1 = platforms[i].start.x;
                float startX2 = platforms[j].start.x;
                float startY1 = platforms[i].start.y;
                float startY2 = platforms[j].start.y;
                float length1 = platforms[i].length;
                float length2 = platforms[j].length;

                float minXDistance = Mathf.Infinity;
                minXDistance = Mathf.Min(minXDistance, Mathf.Abs(startX1 - startX2));
                minXDistance = Mathf.Min(minXDistance, Mathf.Abs(startX1 - (startX2 + length2 - 1.0f)));
                minXDistance = Mathf.Min(minXDistance, Mathf.Abs((startX1 + length1 - 1.0f) - startX2));
                minXDistance = Mathf.Min(minXDistance, Mathf.Abs((startX1 + length1 - 1.0f) - (startX2 + length2 - 1.0f)));

                float yDistance = startY2 - startY1;

                
            }
        }

        drawTrajectory(trajectories[trajectoryView]);
    }

    private float mapRange(float input, float inputStart, float inputEnd, float outputStart, float outputEnd)
    {
        return outputStart + ((outputEnd - outputStart) / (inputEnd - inputStart)) * (input - inputStart);
    }

    private Vector3 tilemapToWorldPos(Vector3 tilemapPos)
    {
        float x = tilemapPos.x + 0.5f;
        float y = tilemapPos.y + 0.5f;
        float z = tilemapPos.z;

        return new Vector3(x, y, z);
    }

    private void handleEdgeCreation(Platform platform0, Platform platform1, ref SurfaceGraph graph)
    {
        if (platform0.start.y == platform1.start.y)
        {
            // Get left and right platforms
            Platform leftPlatform = platform0;
            Platform rightPlatform = platform1;
            if (leftPlatform.start.x > rightPlatform.start.x)
            {
                (leftPlatform, rightPlatform) = (rightPlatform, leftPlatform);
            }

            float x1 = leftPlatform.start.x + leftPlatform.length - 1.0f;
            float y1 = leftPlatform.start.y;
            Vector3 worldPos1 = tilemapToWorldPos(new Vector3(x1, y1, 0.0f));
            x1 = worldPos1.x;
            y1 = worldPos1.y + 0.5f;

            float x2 = rightPlatform.start.x;
            float y2 = rightPlatform.start.y;
            Vector3 worldPos2 = tilemapToWorldPos(new Vector3(x2, y2, 0.0f));
            x2 = worldPos2.x;
            y2 = worldPos2.y + 0.5f;

            JumpTrajectory trajectory = calculateTrajectory(x1, y1, x2, y2);
            trajectories.Add(trajectory);

            
        }
    }

    private float distance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2.0f) + Mathf.Pow(y2 - y1, 2.0f));
    }

    private JumpTrajectory calculateTrajectory(float x1, float y1, float x2, float y2)
    {
        float k = y1 + jumpHeight;

        float d = y2 - y1;

        float f = 2.0f * x1 * y2 - 2.0f * y1 * x2 + 2.0f * k * x2 - 2.0f * k * x1;
        float g = -(Mathf.Pow(x1, 2.0f) * (y2 - k) - Mathf.Pow(x2, 2.0f) * (y1 - k));

        float h1 = (-f + Mathf.Sqrt(Mathf.Pow(f, 2.0f) - 4.0f * d * g)) / (2.0f * d);
        float h2 = (-f - Mathf.Sqrt(Mathf.Pow(f, 2.0f) - 4.0f * d * g)) / (2.0f * d);

        if (d == 0.0f)
        {
            h1 = h2 = (x1 + x2) / 2.0f;
        }

        float a1 = (y1 - k) / (Mathf.Pow(x1, 2.0f) - 2.0f * h1 * x1 + Mathf.Pow(h1, 2.0f));
        float a2 = (y1 - k) / (Mathf.Pow(x1, 2.0f) - 2.0f * h2 * x1 + Mathf.Pow(h2, 2.0f));

        float b1 = -2.0f * a1 * h1;
        float b2 = -2.0f * a2 * h2;

        float c1 = a1 * Mathf.Pow(h1, 2.0f) + k;
        float c2 = a2 * Mathf.Pow(h2, 2.0f) + k;

        if (h1 >= x1 && h1 <= x2)
        {
            return new JumpTrajectory(a1, b1, c1, x1, x2);
        }
        else
        {
            return new JumpTrajectory(a2, b2, c2, x1, x2);
        }
    }

    private void drawTrajectory(JumpTrajectory trajectory)
    {
        float x1 = trajectory.x1;
        float x2 = trajectory.x2;

        float step = (x2 - x1) / 50.0f;
        for (int i = 0; i < 51; i++)
        {
            float x = x1 + i * step;
            float y = trajectory.a * Mathf.Pow(x, 2.0f) + trajectory.b * x + trajectory.c;
            //Debug.Log(trajectory.toString());
            GameObject gameObject = Instantiate(trajectoryBrush, new Vector3(x, y, 0.0f), Quaternion.identity);
            brushes.Add(gameObject);
        }
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

                float shade = (float)x / platform.length;

                highlightATile(highlightPos, new Color(shade, shade, shade, 1.0f));
            }
        }
    }

    private void highlightATile(Vector3Int highlightPos, Color color)
    {
        highlightTilemap.SetTile(highlightPos, highlightTile);
        highlightTilemap.SetTileFlags(highlightPos, TileFlags.None);

        highlightTilemap.SetColor(highlightPos, color);
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
