using System.Collections.Generic;
using UnityEngine;

public class BugMovementNode : KiwiActionNode
{
    public float speed = 5;
    public float turnSpeed = 3;
    public float turnDist = 5;
    public float stoppingDist = 1f;
    public float minPathUpdateTime = .2f;

    public WalkingGrid walkingGrid;
    public GameObject agentObj;

    private DrawingPath path;
    private Vector3 targetPos;
    private int pathIndex = 0;
    private float lastPathUpdateTime;
    private bool pathSuccessful;

    private bool isInit = false;

    protected override void OnStart()
    {
        if (!isInit)
        {
            walkingGrid = blackboard.walkingGrid;
            agentObj = blackboard.agentObj;

            isInit = true;
        }

        PickRandomTarget();
        RequestNewPath();
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (path == null)
            return State.Running;

        if (Time.time - lastPathUpdateTime > minPathUpdateTime)
        {
            RequestNewPath();
        }

        Vector2 pos2D = new Vector2(agentObj.transform.position.x, agentObj.transform.position.z);

        while (pathIndex <= path.finishLineIndex && path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
        {
            pathIndex++;
        }

        if (pathIndex > path.finishLineIndex)
        {
            return State.Success;
        }

        Vector3 targetPoint = path.lookPoints[Mathf.Min(pathIndex, path.lookPoints.Length - 1)];
        Vector3 directionToTarget = targetPoint - agentObj.transform.position;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(directionToTarget);
            agentObj.transform.rotation = Quaternion.Lerp(agentObj.transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }

        agentObj.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        return State.Running;
    }

    void RequestNewPath()
    {
        lastPathUpdateTime = Time.time;
        PathRequestManager.RequestPath(new PathRequest(agentObj.transform.position, targetPos, OnPathFound));
    }

    public void OnPathFound(Vector3[] waypoints, bool success)
    {
        pathSuccessful = success;
        if (success)
        {
            path = new DrawingPath(waypoints, agentObj.transform.position, turnDist, stoppingDist);
            pathIndex = 0;
        }
    }

    void PickRandomTarget()
    {
        List<WalkingNode> walkableNodes = new List<WalkingNode>();
        for (int x = 0; x < walkingGrid.gridSizeX; x++)
        {
            for (int y = 0; y < walkingGrid.gridSizeY; y++)
            {
                if (walkingGrid.grid[x, y].walkable)
                    walkableNodes.Add(walkingGrid.grid[x, y]);
            }
        }

        if (walkableNodes.Count > 0)
        {
            targetPos = walkableNodes[Random.Range(0, walkableNodes.Count)].worldPosition;
        }
    }
}