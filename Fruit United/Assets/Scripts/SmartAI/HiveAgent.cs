using System.Collections.Generic;
using System.Linq;
using QuikGraph.Algorithms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Edge = QuikGraph.TaggedEdge<Platform, TrajectoryBundle>;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.TaggedEdge<Platform, TrajectoryBundle>>;

public class HiveAgent
{
    public int id;
    private AgentAttributes attributes;
    private Brain brain;
    private Entity user;

    private HiveDrawingTools hiveDrawingTools;

    private Platform startPlatform;
    private Platform endPlatform;
    private List<Edge> edges;
    private List<Trajectory> trajectories;
    private List<JumpTrajectory> jumpTrajectories;
    private List<RunTrajectory> runTrajectories;
    private List<int> jumpTrajectoryIndices;
    private List<int> runTrajectoryIndices;

    private System.Diagnostics.Stopwatch stopwatch;
    private double previousElapsed;

    bool followingTrajectory = false;
    private int currentTrajectoryIndex = 0;
    private float previousXPos = 0.0f;
    private float currentRBVelocity = 0.0f;
    private float jumpX = 0.0f;
    public HiveAgent(int id, AgentAttributes attributes, Brain brain, Entity user, HiveDrawingTools hiveDrawingTools)
    {
        this.id = id;
        this.attributes = attributes;
        this.brain = brain;
        this.user = user;

        this.hiveDrawingTools = hiveDrawingTools;

        trajectories = new List<Trajectory>();
        jumpTrajectories = new List<JumpTrajectory>();
        runTrajectories = new List<RunTrajectory>();
        jumpTrajectoryIndices = new List<int>();
        runTrajectoryIndices = new List<int>();

        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Restart();
    }

    public void TakeControl()
    {
        if (!followingTrajectory)
        {
            GoToRandomPlace();
            return;
        }

        //Debug.Log(currentTrajectoryIndex);

        Trajectory trajectory = trajectories[currentTrajectoryIndex];
        Transform transform = user.transform;

        double elapsed = stopwatch.Elapsed.TotalSeconds;
        double deltaTime = elapsed - previousElapsed;
        previousElapsed = elapsed;

        if (trajectory is JumpTrajectory)
        {
            JumpTrajectory jumpTrajectory = (JumpTrajectory) trajectory;
            float speed = Mathf.Sqrt(attributes.jumpA / jumpTrajectory.a);
            if (float.IsNaN(speed))
            {
                Debug.Log(jumpTrajectory.toString());
            }
            float xDir = Mathf.Sign(jumpTrajectory.x2 - jumpTrajectory.x1);
            
            jumpX += (float)(speed * xDir * deltaTime);

            float xPos = jumpX;
            float yPos = jumpTrajectory.plugIn(xPos);

            transform.position = new Vector3(xPos, yPos, 0.0f);

            float jumpProgress = mapRange(jumpX, jumpTrajectory.x1, jumpTrajectory.x2, 0.0f, 1.0f);
            if (jumpProgress > 1.0f)
            {
                transform.position = new Vector3(jumpTrajectory.x2, jumpTrajectory.y2, 0.0f);
                user.AddComponent<Rigidbody2D>();
                user.rb = user.GetComponent<Rigidbody2D>();
                user.rb.linearVelocityX = xDir * speed;
                //user.rb.linearVelocityX = 0.0f;
                user.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                currentTrajectoryIndex++;
            }
        }
        else
        {
            RunTrajectory runTrajectory = (RunTrajectory)trajectory;
            if (currentTrajectoryIndex == trajectories.Count - 1)
            {
                throw new System.Exception("No jump trajectory after run trajectory");
            }
            JumpTrajectory nextTrajectory = (JumpTrajectory)trajectories[currentTrajectoryIndex + 1];



            //float xDir = Mathf.Sign(runTrajectory.x2 - runTrajectory.x1);
            float xDir = Mathf.Sign(runTrajectory.x2 - transform.position.x);

            float velocity = (float)((transform.position.x - previousXPos) / deltaTime);
            currentRBVelocity = user.rb.linearVelocityX;
            previousXPos = transform.position.x;
            
            if (Mathf.Abs(runTrajectory.x2 - transform.position.x) > 1.0f)
            {
                if (Mathf.Abs(user.rb.linearVelocityX) < user.maxSpeedX)
                {
                    user.rb.AddForce(new Vector2(xDir * user.xAccel, 0.0f), ForceMode2D.Force);
                }
            }
            else
            {
                if (velocity < xDir * nextTrajectory.idealSpeed)
                {
                    if (Mathf.Abs(user.rb.linearVelocityX) < user.maxSpeedX)
                    {
                        user.rb.AddForce(new Vector2(user.xAccel, 0.0f), ForceMode2D.Force);
                    }
                }
                else
                {
                    if (Mathf.Abs(user.rb.linearVelocityX) < user.maxSpeedX)
                    {
                        user.rb.AddForce(new Vector2(-user.xAccel, 0.0f), ForceMode2D.Force);
                    }
                }
            }

            float progress = mapRange(transform.position.x + (float)(user.rb.linearVelocityX * deltaTime), runTrajectory.x1, runTrajectory.x2, 0.0f, 1.0f);

            if (progress > 1.0f)
            {
                currentTrajectoryIndex++;

                Object.Destroy(user.GetComponent<Rigidbody2D>());
                jumpX = nextTrajectory.x1;
            }
        }

        if (currentTrajectoryIndex >= trajectories.Count)
        {
            followingTrajectory = false;
            currentTrajectoryIndex = 0;
            currentRBVelocity = 0.0f;
            previousXPos = 0.0f;
            jumpX = 0.0f;
        }
    }

    public void GoToRandomPlace()
    {
        hiveDrawingTools.ClearVisuals();

        List<Platform> platforms = brain.Platforms;
        SurfaceGraph graph = brain.Graph;

        IEnumerable<Edge> edgeInterface = null;
        System.Random random = new System.Random();

        Platform platform0 = GetCurrentPlatform();
        Platform platform1 = platforms[1];

        int iterations = 0;
        while (edgeInterface == null && iterations < 100)
        {
            platform1 = platforms[(int)(random.NextDouble() * platforms.Count)];

            graph.ShortestPathsAStar((Edge edge) => 1.0, (Platform p) => 1.0, platform0)(platform1, out edgeInterface);

            iterations++;
        }

        if (edgeInterface == null)
        {
            Debug.Log("No paths from platform [" + platform0.toString() + "] to platform [" + platform1.toString() + "]");
            return;
        }

        startPlatform = platform0;
        endPlatform = platform1;
        edges = edgeInterface.ToList();

        LinkTrajectories();
        followingTrajectory = true;
        currentTrajectoryIndex = 0;
    }

    public void GoToPlatform(float x1, float y1)
    {
        hiveDrawingTools.ClearVisuals();

        List<Platform> platforms = brain.Platforms;
        SurfaceGraph graph = brain.Graph;

        IEnumerable<Edge> edgeInterface = null;
        System.Random random = new System.Random();

        Platform platform0 = GetCurrentPlatform();
        Platform platform1 = null;

        foreach (Platform platform in platforms)
        {
            if (platform.start.x == x1 && platform.start.y == y1)
            {
                platform1 = platform;
                break;
            }
        }

        if (platform1 == null)
        {
            Debug.Log("Couldn't find platform with X1: " + x1 + " Y1: " + y1);
            return;
        }

        graph.ShortestPathsAStar((Edge edge) => 1.0, (Platform p) => 1.0, platform0)(platform1, out edgeInterface);

        if (edgeInterface == null)
        {
            Debug.Log("No paths from platform [" + platform0.toString() + "] to platform [" + platform1.toString() + "]");
            return;
        }

        startPlatform = platform0;
        endPlatform = platform1;
        edges = edgeInterface.ToList();

        LinkTrajectories();
        followingTrajectory = true;
        currentTrajectoryIndex = 0;
    }

    private void LinkTrajectories()
    {
        //Debug.Log("Start: " + startPlatform.toString());
        //Debug.Log("End: " + endPlatform.toString());
        //Debug.Log("Jumps: " + edges.Count);
        //hiveDrawingTools.HighlightPlatform(startPlatform, (_) => new Color(0.0f, 1.0f, 0.0f, 1.0f));
        //hiveDrawingTools.HighlightPlatform(endPlatform, (_) => new Color(0.0f, 0.0f, 1.0f, 1.0f));

        //for (int i = 0; i < edges.Count; i++)
        //{
        //    Edge edge = edges[i];
        //    Debug.Log("Jump " + i + ": " + edge.Tag.world.toString());
        //}



        trajectories.Clear();
        jumpTrajectories.Clear();
        runTrajectories.Clear();

        float x1 = user.transform.position.x;
        float y1 = startPlatform.start.y + 0.5f;

        int trajectoryIndex = 0;
        foreach (Edge edge in edges)
        {
            float x2 = edge.Tag.world.x1;
            float y2 = edge.Tag.world.y1;

            RunTrajectory worldTrajectory = new RunTrajectory(x1, y1, x2, y2);

            runTrajectories.Add(worldTrajectory);
            trajectories.Add(worldTrajectory);
            runTrajectoryIndices.Add(trajectoryIndex);

            trajectoryIndex++;

            jumpTrajectories.Add(edge.Tag.world);
            trajectories.Add(edge.Tag.world);
            jumpTrajectoryIndices.Add(trajectoryIndex);

            trajectoryIndex++;

            x1 = edge.Tag.world.x2;
            y1 = edge.Tag.world.y2;
        }

        //foreach (RunTrajectory runTrajectory in runTrajectories)
        //{
        //    hiveDrawingTools.DrawRunTrajectory(runTrajectory);
        //}

        //foreach (Edge edge in edges)
        //{
        //    hiveDrawingTools.DrawJumpTrajectory(edge.Tag.tilemap, edge.Tag.world, attributes);
        //}
    }

    private Platform GetCurrentPlatform()
    {
        List<Platform> platforms = brain.Platforms;
        Debug.Log("Platform count: " + platforms.Count);
        Transform transform = user.transform;
        Debug.Log("Transform position: " + transform.position);

        float lowestY = Mathf.Infinity;
        Platform currentPlatform = null;
        foreach (Platform platform in platforms)
        {
            if (platform.x1 + 0.5f > transform.position.x + attributes.width / 2.0f || platform.x2 + 0.5f < transform.position.x - attributes.width / 2.0f)
            {
                continue;
            }

            if (platform.start.y > transform.position.y)
            {
                continue;
            }

            if (transform.position.y - platform.start.y < lowestY)
            {
                lowestY = transform.position.y - platform.start.y;
                currentPlatform = platform;
            }
        }

        if (currentPlatform == null)
        {
            throw new System.Exception("The agent is not on a platform currently.");
        }

        return currentPlatform;
    }

    private void moveUser()
    {

    }
    private float mapRange(float input, float inputStart, float inputEnd, float outputStart, float outputEnd)
    {
        return outputStart + ((outputEnd - outputStart) / (inputEnd - inputStart)) * (input - inputStart);
    }
}