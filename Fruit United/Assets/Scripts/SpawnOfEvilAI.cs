using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using QuikGraph;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.Edge<Platform>>;
using System.Linq;

public class SpawnOfEvilAI : MonoBehaviour
{

    public Tilemap tilemap;

    public Tilemap highlightTilemap;
    public Tile highlightTile;
    public GameObject trajectoryBrush;
    public int agentHeight = 1;

    public float jumpHeight = 5.0f;

    private List<JumpTrajectory> worldTrajectories;
    private List<JumpTrajectory> tilemapTrajectories;

    public int trajectoryView = 0;

    private List<GameObject> brushes = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldTrajectories = new List<JumpTrajectory>();
        tilemapTrajectories = new List<JumpTrajectory>();

    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject go in brushes)
        {
            Destroy(go);
        }
        worldTrajectories.Clear();


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
        // printPlatforms(platforms);

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

        drawTrajectory(trajectoryView);
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

    private Vector3 worldToTilemapPos(Vector3 worldPos)
    {
        float x = worldPos.x - 0.5f;
        float y = worldPos.y - 0.5f;
        float z = worldPos.z;

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
            float y1 = leftPlatform.start.y + 1.0f;
            Vector3 worldPos1 = tilemapToWorldPos(new Vector3(x1, y1, 0.0f));
            float x2 = rightPlatform.start.x;
            float y2 = rightPlatform.start.y + 1.0f;
            Vector3 worldPos2 = tilemapToWorldPos(new Vector3(x2, y2, 0.0f));

            JumpTrajectory tilemapTrajectory = calculateTrajectory(x1, y1, x2, y2);

            x1 = worldPos1.x;
            y1 = worldPos1.y;
            x2 = worldPos2.x;
            y2 = worldPos2.y;

            JumpTrajectory worldTrajectory = calculateTrajectory(x1, y1, x2, y2);

            List<Vector3Int> crossedTiles = crawlTrajectory(tilemapTrajectory);
            bool spaceFree = true;
            foreach (Vector3Int crossedTile in crossedTiles)
            {
                if (tilemap.HasTile(crossedTile))
                {
                    spaceFree = false;
                }
            }

            if (!spaceFree)
            {
                return;
            }

            tilemapTrajectories.Add(tilemapTrajectory);
            worldTrajectories.Add(worldTrajectory);
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

    private List<Vector3Int> crawlTrajectory(JumpTrajectory trajectory)
    {
        HashSet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

        int xStart = (int)trajectory.x1;
        int xEnd = (int)trajectory.x2;

        // Debug.Log("Debug Start");

        //float x1 = xStart;
        //float y1 = getYFromTrajectory(trajectory, xStart);
        //for (int i = xStart + 1; i <= xEnd; i++)
        //{
        //    float x2 = i;
        //    float y2 = getYFromTrajectory(trajectory, x2);

        //    //Vector3 pos1 = worldToTilemapPos(new Vector3(x1, y1, 0.0f));
        //    //Vector3 pos2 = worldToTilemapPos(new Vector3(x2, y2, 0.0f));
        //    //crawlLine(ref tilePositions, pos1.x, pos1.y, pos2.x, pos2.y);


        //    crawlLine(ref tilePositions, x1, y1, x2, y2);

        //    x1 = x2;
        //    y1 = y2;
        //}

        for (float i = xStart; i <= xEnd; i += 0.1f)
        {
            float fx = i;
            float fy = getYFromTrajectory(trajectory, fx);

            int x = Mathf.RoundToInt(fx);
            int y = Mathf.RoundToInt(fy);

            tilePositions.Add(new Vector3Int(x, y, 0));
        }

        List<Vector3Int> tileList = new List<Vector3Int>(tilePositions.ToArray());

        return tileList;
    }

    private float crawlProgress(float start, float finish, float value)
    {
        if (start == finish)
            return 1.0f;
        else if (Mathf.Sign(finish - start) != Mathf.Sign(finish - value))
            return 1.0f;
        else
            return (value - start) / (finish - start);
    }

    private void crawlLine(ref HashSet<Vector3Int> tilePositions, float fx1, float fy1, float fx2, float fy2)
    {
        int x1 = Mathf.RoundToInt(fx1);
        int y1 = Mathf.RoundToInt(fy1);
        int x2 = Mathf.RoundToInt(fx2);
        int y2 = Mathf.RoundToInt(fy2);

        int xDir = x2 > x1 ? 1 : -1;
        int xCurrent = x1;
        float xProgress = crawlProgress(x1, x2, xCurrent);

        int yDir = y2 > y1 ? 1 : -1;
        int yCurrent = y1;
        float yProgress = crawlProgress(y1, y2, yCurrent);

        tilePositions.Add(new Vector3Int(xCurrent, yCurrent, 0));

        while (xProgress < 1.0f || yProgress < 1.0f)
        {
            bool shouldMoveX = xProgress <= yProgress;
            bool shouldMoveY = yProgress <= xProgress;

            if (shouldMoveX && shouldMoveY)
            {
                float tempXProgress = crawlProgress(x1, x2, xCurrent + 0.1f * xDir);
                float tempYProgress = crawlProgress(y1, y2, yCurrent + 0.1f * yDir);
                shouldMoveX = tempXProgress <= tempYProgress;
                shouldMoveY = tempYProgress <= tempXProgress;

            }

            if (shouldMoveX)
            {
                xCurrent += xDir;
                xProgress = crawlProgress(x1, x2, xCurrent);
            }
            if (shouldMoveY)
            {
                yCurrent += yDir;
                yProgress = crawlProgress(y1, y2, yCurrent);
            }

            tilePositions.Add(new Vector3Int(xCurrent, yCurrent, 0));
        }
    }

    private float getYFromTrajectory(JumpTrajectory trajectory, float x)
    {
        return trajectory.a * Mathf.Pow(x, 2.0f) + trajectory.b * x + trajectory.c;
    }

    private void drawTrajectory(int trajectoryIndex)
    {
        JumpTrajectory worldTrajectory = worldTrajectories[trajectoryIndex];
        JumpTrajectory tilemapTrajectory = tilemapTrajectories[trajectoryIndex];

        float x1 = worldTrajectory.x1;
        float x2 = worldTrajectory.x2;

        float step = (x2 - x1) / 50.0f;
        for (int i = 0; i < 51; i++)
        {
            float x = x1 + i * step;
            float y = getYFromTrajectory(worldTrajectory, x);
            //Debug.Log(trajectory.toString());
            GameObject gameObject = Instantiate(trajectoryBrush, new Vector3(x, y, 0.0f), Quaternion.identity);
            brushes.Add(gameObject);
        }

        List<Vector3Int> tileList = crawlTrajectory(tilemapTrajectory);

        foreach (Vector3Int pos in tileList)
        {
            highlightATile(pos, new Color(1.0f, 0.0f, 0.0f, 0.5f));
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
