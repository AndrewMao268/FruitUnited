using System.Collections.Generic;
using Edge = QuikGraph.TaggedEdge<Platform, TrajectoryBundle>;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.TaggedEdge<Platform, TrajectoryBundle>>;

public class TrajectoryPath
{
    List<JumpTrajectory> jumpTrajectories;
    List<RunTrajectory> runTrajectories;
    

    public TrajectoryPath(List<Edge> edges)
    {
        jumpTrajectories = new List<JumpTrajectory>();
        runTrajectories = new List<RunTrajectory>();



        foreach (Edge edge in edges)
        {
            jumpTrajectories.Add(edge.Tag.world);
        }
    }
}