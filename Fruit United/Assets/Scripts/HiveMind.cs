using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph.Algorithms;
using UnityEngine;
using UnityEngine.Tilemaps;
using Edge = QuikGraph.TaggedEdge<Platform, TrajectoryBundle>;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.TaggedEdge<Platform, TrajectoryBundle>>;


public class HiveMind : MonoBehaviour
{

    public Tilemap tilemap;
    public Tilemap highlightTilemap;
    public Tile highlightTile;
    public GameObject trajectoryBrush;
    private List<GameObject> brushes;
    private HiveDrawingTools hiveDrawingTools;

    private int nextAgentId;
    private List<AgentAttributes> agentAttributes;
    private List<Brain> brains;

    private List<HiveAgent> agents;

    void Start()
    {
        brushes = new List<GameObject>();
        hiveDrawingTools = new HiveDrawingTools(highlightTilemap, highlightTile, trajectoryBrush, brushes);

        nextAgentId = 0;
        agentAttributes = new List<AgentAttributes>();
        brains = new List<Brain>();
        agents = new List<HiveAgent>();
    }

    public int RegisterAgent(AgentAttributes newAttributes, Entity user)
    {
        int brainId = 0;

        bool isNew = true;
        for (int i = 0; i < agentAttributes.Count; i++)
        {
            AgentAttributes attributes = agentAttributes[i];
            if (attributes.isSame(newAttributes))
            {
                isNew = false;
                brainId = i;
            }
        }

        if (isNew)
        {
            agentAttributes.Add(newAttributes);
            brains.Add(new Brain(newAttributes, tilemap, hiveDrawingTools));
            brainId = brains.Count - 1;
        }

        int agentId = nextAgentId;
        nextAgentId++;
        agents.Add(new HiveAgent(agentId, newAttributes, brains[brainId], user, hiveDrawingTools));

        return agentId;
    }

    public void GoToRandomPlace(int agentId)
    {
        if (agentId < 0 || agentId >= agents.Count)
        {
            throw new ArgumentOutOfRangeException("Invalid Agent ID " + agentId);
        }

        HiveAgent agent = agents[agentId];
        agent.GoToRandomPlace();
    }

    public void GoToPlatform(int agentId, float x1, float y1)
    {
        if (agentId < 0 || agentId >= agents.Count)
        {
            throw new ArgumentOutOfRangeException("Invalid Agent ID " + agentId);
        }

        HiveAgent agent = agents[agentId];
        agent.GoToPlatform(x1, y1);
    }

    public void SurrenderControl(int agentId)
    {
        HiveAgent agent = agents[agentId];
        agent.TakeControl();
    }
}