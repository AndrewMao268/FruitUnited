using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph.Algorithms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Edge = QuikGraph.TaggedEdge<Platform, TrajectoryBundle>;
using SurfaceGraph = QuikGraph.AdjacencyGraph<Platform, QuikGraph.TaggedEdge<Platform, TrajectoryBundle>>;

public class SpawnOfEvilAI : Entity
{
    public GameObject hiveGameObject;
    private HiveMind hiveMind;

    AgentAttributes agentAttributes;
    int agentId;
    bool registered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void OwnStart()
    {
        maxSpeedX = 5.0f;
        xAccel = 20.0f;

        hiveMind = (HiveMind)hiveGameObject.GetComponent<MonoBehaviour>();

        agentAttributes = new AgentAttributes();
        agentAttributes.height = GetComponent<BoxCollider2D>().bounds.size.y;
        agentAttributes.width = GetComponent<BoxCollider2D>().bounds.size.x;
        agentAttributes.jumpHeight = 3.0f;
        agentAttributes.maxSpeed = maxSpeedX;
        agentAttributes.jumpA = -4.89161183078935f;
        agentAttributes.jumpB = 7.71168716787364f;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!registered)
        {
            agentId = hiveMind.RegisterAgent(agentAttributes, this);

            float goalX = Mathf.Round(GameObject.Find("Goal").transform.position.x - 0.5f);
            float goalY = Mathf.Round(GameObject.Find("Goal").transform.position.y - 0.5f);
            hiveMind.GoToPlatform(agentId, goalX, goalY);
            registered = true;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hiveMind.GoToRandomPlace(agentId);
            }
        }
    }

    protected override void OwnFixedUpdate()
    {
        grounded = Physics2D.OverlapCapsule(feet.position, new Vector2(capsuleX, capsuleY), CapsuleDirection2D.Horizontal, 0, groundLayer);

        if (registered)
        {
            hiveMind.SurrenderControl(agentId);
        }

        previousGround = grounded;
    }
}
