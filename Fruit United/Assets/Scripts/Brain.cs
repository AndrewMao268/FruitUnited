using System.Collections.Generic;
using System.Linq;
using QuikGraph.Algorithms;
using UnityEngine;
using UnityEngine.Tilemaps;
using Edge = QuikGraph.TaggedEdge<Platform, TrajectoryBundle>;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.TaggedEdge<Platform, TrajectoryBundle>>;

public class Brain
{
    private AgentAttributes attributes;

    private Tilemap tilemap;
    private HiveDrawingTools hiveDrawingTools;

    private SurfaceGraph graph;
    private List<Platform> platforms;
    private List<JumpTrajectory> worldTrajectories;
    private List<JumpTrajectory> tilemapTrajectories;

    private System.Diagnostics.Stopwatch stopwatch;

    public Brain(AgentAttributes attributes, Tilemap tilemap, HiveDrawingTools hiveDrawingTools)
    {
        this.attributes = attributes;

        this.tilemap = tilemap;
        this.hiveDrawingTools = hiveDrawingTools;

        stopwatch = new System.Diagnostics.Stopwatch();

        graph = new SurfaceGraph(true);
        platforms = new List<Platform>();
        worldTrajectories = new List<JumpTrajectory>();
        tilemapTrajectories = new List<JumpTrajectory>();

        FindPlatforms();
        FindTrajectories();
    }

    private void FindPlatforms()
    {
        int z = tilemap.cellBounds.z;

        int lowestY = tilemap.cellBounds.min.y;
        int highestY = tilemap.cellBounds.max.y;

        int lowestX = tilemap.cellBounds.min.x;
        int highestX = tilemap.cellBounds.max.x;

        platforms.Clear();

        for (int y = lowestY; y <= highestY; y++)
        {
            //Debug.Log("Current y: " + y);
            int x = lowestX - 1;
            SkipTiles(ref x, y, z);
            //Debug.Log("Initial x: " + x);

            while (x <= highestX)
            {
                int startX = x;
                MeasureTiles(ref x, y, z);
                int platformLength = x - startX;

                Platform platform = new Platform(new Vector3Int(startX, y, z), platformLength);
                platforms.Add(platform);
                //Debug.Log("New platform created!" + platform.toString());

                SkipTiles(ref x, y, z);
            }
        }

        //Debug.Log("lowestY: " + lowestX);
        //Debug.Log("highestY: " + highestX);

        foreach (Platform platform in platforms)
        {
            hiveDrawingTools.HighlightPlatform(platform);
        }
    }

    private void SkipTiles(ref int x, int y, int z)
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

            if (!EnoughSpace(x, y, z))
            {
                continueSkipping = true;
            }

            if (x > highestX)
            {
                continueSkipping = false;
            }
        }
    }
    private void MeasureTiles(ref int x, int y, int z)
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

            if (!EnoughSpace(x, y, z))
            {
                continueMeasuring = false;
            }

            if (x > highestX)
            {
                continueMeasuring = false;
            }
        }
    }
    private bool EnoughSpace(int x, int y, int z)
    {
        for (int airY = 1; airY <= Mathf.CeilToInt(attributes.height); airY++)
        {
            if (tilemap.HasTile(new Vector3Int(x, y + airY, z)))
            {
                return false;
            }
        }

        return true;
    }
    private void FindTrajectories()
    {
        graph.Clear();

        for (int i = 0; i < platforms.Count; i++)
        {
            graph.AddVertex(platforms[i]);
        }

        for (int i = 0; i < platforms.Count; i++)
        {
            for (int j = i + 1; j < platforms.Count; j++)
            {
                CreateTrajectories(platforms[i], platforms[j], ref graph);
            }
        }

        Debug.Log("Trajectory Count: " + tilemapTrajectories.Count);

        //indices = Enumerable.Range(0, tilemapTrajectories.Count - 1).ToList();
        //indices.Sort((int a, int b) => (int)((Mathf.Abs(tilemapTrajectories[a].a) - Mathf.Abs(tilemapTrajectories[b].a)) * 1000.0f));
    }

    private void CreateTrajectories(Platform platform0, Platform platform1, ref SurfaceGraph graph)
    {
        float x1;
        float y1;
        float x2;
        float y2;

        if (platform0.start.y < platform1.start.y)
        {
            (platform0, platform1) = (platform1, platform0);
        }

        // 0.01f is for slight errors in calculations
        float leftLedge0 = platform0.x1 - attributes.width / 2.0f + 0.01f;
        float rightLedge0 = platform0.x2 + attributes.width / 2.0f + 0.01f;

        bool succeededForward = false;
        bool succeededBackward = false;

        for (int j = 0; j < platform1.length; j++)
        {
            x1 = leftLedge0;
            y1 = platform0.start.y + 1.0f;
            x2 = platform1.start.x + j;
            y2 = platform1.start.y + 1.0f;

            succeededForward = succeededForward || CreateTrajectories(x1, y1, x2, y2, platform0, platform1, ref graph);

            x1 = rightLedge0;

            succeededForward = succeededForward || CreateTrajectories(x1, y1, x2, y2, platform0, platform1, ref graph);
        }


        for (int i = 0; i < platform0.length; i++)
        {
            for (int j = 0; j < platform1.length; j++)
            {
                x1 = platform0.start.x + i;
                y1 = platform0.start.y + 1.0f;
                x2 = platform1.start.x + j;
                y2 = platform1.start.y + 1.0f;

                if (!succeededForward)
                {
                    succeededForward = succeededForward || CreateTrajectories(x1, y1, x2, y2, platform0, platform1, ref graph);
                }
                
                if (!succeededBackward)
                {
                    succeededBackward = succeededBackward || CreateTrajectories(x2, y2, x1, y1, platform1, platform0, ref graph);
                }
            }
        }
    }

    private bool CreateTrajectories(float x1, float y1, float x2, float y2, Platform platform0, Platform platform1, ref SurfaceGraph graph)
    {
        for (float i = 0.0f; i <= attributes.jumpHeight; i++)
        {
            if (CreateTrajectory(x1, y1, x2, y2, i, platform0, platform1, ref graph))
            {
                return true;
            }
        }

        return false;

        //if (jump)
        //{
        //    return CreateTrajectory(x1, y1, x2, y2, attributes.jumpHeight, platform0, platform1, ref graph);
        //}
        //else
        //{
        //    return CreateTrajectory(x1, y1, x2, y2, 0.0f, platform0, platform1, ref graph);
        //}
    }

    private bool CreateTrajectory(float x1, float y1, float x2, float y2, float jumpHeight, Platform platform0, Platform platform1, ref SurfaceGraph graph)
    {
        JumpTrajectory tilemapTrajectory = GenerateJumpTrajectory(x1, y1, x2, y2, jumpHeight, platform0, platform1);

        if (!VerifyTrajectory(tilemapTrajectory, platform0, platform1))
        {
            return false;
        }

        Vector3 worldPos1 = TilemapToWorldPos(new Vector3(x1, y1, 0.0f));
        Vector3 worldPos2 = TilemapToWorldPos(new Vector3(x2, y2, 0.0f));

        x1 = worldPos1.x;
        y1 = worldPos1.y;
        x2 = worldPos2.x;
        y2 = worldPos2.y;

        JumpTrajectory worldTrajectory = GenerateJumpTrajectory(x1, y1, x2, y2, jumpHeight, platform0, platform1);

        tilemapTrajectories.Add(tilemapTrajectory);
        worldTrajectories.Add(worldTrajectory);

        bool success = graph.AddEdge(new Edge(platform0, platform1, new TrajectoryBundle(tilemapTrajectory, worldTrajectory)));
        if (!success)
        {
            Debug.Log("Not a success!");
        }

        return true;
    }

    private JumpTrajectory GenerateJumpTrajectory(float x1, float y1, float x2, float y2, float jumpHeight, Platform platform0, Platform platform1)
    {
        float k = y1 + jumpHeight;

        if (jumpHeight == 0.0f)
        {
            float h = x1;

            float a = (y2 - k) / (Mathf.Pow(x2, 2.0f) - 2.0f * h * x2 + Mathf.Pow(h, 2.0f));
            float b = -2.0f * a * h;
            float c = a * Mathf.Pow(h, 2.0f) + k;

            float idealSpeed = Mathf.Sqrt(attributes.jumpA / a);

            return new JumpTrajectory(a, b, c, x1, y1, x2, y2, jumpHeight, platform0, platform1, idealSpeed);
        }

        float d = y1 - y2;

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

        //if (float.IsNaN(a1) || float.IsNaN(a2))
        //{
        //    string debugStr = "X1: " + x1 + " Y1: " + y1 + " X2: " + x2 + " Y2: " + y2 + " Jump Height: " + jumpHeight;
        //    debugStr += "\n" + "D: " + d + " F: " + f + " G: " + g + " H1: " + h1 + " H2: " + h2;
        //    Debug.Log(debugStr);
        //}

        float b1 = -2.0f * a1 * h1;
        float b2 = -2.0f * a2 * h2;

        float c1 = a1 * Mathf.Pow(h1, 2.0f) + k;
        float c2 = a2 * Mathf.Pow(h2, 2.0f) + k;

        float idealSpeed1 = Mathf.Sqrt(attributes.jumpA / a1);
        float idealSpeed2 = Mathf.Sqrt(attributes.jumpA / a2);

        if (h1 >= x1 && h1 <= x2)
        {
            return new JumpTrajectory(a1, b1, c1, x1, y1, x2, y2, jumpHeight, platform0, platform1, idealSpeed1);
        }
        else
        {
            return new JumpTrajectory(a2, b2, c2, x1, y1, x2, y2, jumpHeight, platform0, platform1, idealSpeed2);
        }
    }
    
    private bool VerifyTrajectory(JumpTrajectory trajectory, Platform platform0, Platform platform1)
    {
        if (trajectory.x1 == trajectory.x2)
        {
            return false;
        }

        if (trajectory.y2 - trajectory.y1 > trajectory.jumpHeight)
        {
            return false;
        }

        if (trajectory.jumpHeight == 0.0f && trajectory.y2 - trajectory.y1 == trajectory.jumpHeight)
        {
            return false;
        }

        if (trajectory.idealSpeed > attributes.maxSpeed)
        {
            return false;
        }

        List<Vector3Int> crossedTiles = CrawlTrajectory(trajectory);
        crossedTiles.Remove(new Vector3Int(Mathf.RoundToInt(trajectory.x1), Mathf.RoundToInt(trajectory.y1), 0));
        crossedTiles.Remove(new Vector3Int(Mathf.RoundToInt(trajectory.x2), Mathf.RoundToInt(trajectory.y2), 0));

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
            return false;
        }

        return true;
    }

    private List<Vector3Int> CrawlTrajectory(JumpTrajectory trajectory)
    {
        HashSet<Vector3Int> tilePositions = new HashSet<Vector3Int>();

        float xStart = trajectory.x1;
        float xEnd = trajectory.x2;

        if (xStart > xEnd)
        {
            (xStart, xEnd) = (xEnd, xStart);
        }

        for (float i = xStart; i <= xEnd; i += Mathf.Min((xEnd - xStart) / 50.0f, 0.1f))
        {
            float fx = i;
            float fy = trajectory.plugIn(fx);

            for (float j = -1; j < 2; j++)
            {
                for (float k = -1; k < 2; k++)
                {
                    float testX = fx + attributes.width / 4.0f * j;
                    float testY = fy + attributes.height / 4.0f * k;

                    int x = Mathf.RoundToInt(testX);
                    int y = Mathf.RoundToInt(testY);

                    tilePositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        List<Vector3Int> tileList = new List<Vector3Int>(tilePositions.ToArray());

        return tileList;
    }

    private Vector3 TilemapToWorldPos(Vector3 tilemapPos)
    {
        float x = tilemapPos.x + 0.5f;
        float y = tilemapPos.y + 0.5f;
        float z = tilemapPos.z;

        return new Vector3(x, y, z);
    }

    private Vector3 WorldToTilemapPos(Vector3 worldPos)
    {
        float x = worldPos.x - 0.5f;
        float y = worldPos.y - 0.5f;
        float z = worldPos.z;

        return new Vector3(x, y, z);
    }

    public List<Platform> Platforms
    {
        get => platforms;
    }

    public SurfaceGraph Graph
    {
        get => graph;
    }
}