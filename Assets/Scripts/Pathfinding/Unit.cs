using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public WalkingGrid walkingGrid;

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    public float speed = 5;
    public float turnSpeed = 3;
    public float turnDist = 5;
    public float stoppingDist = 1f;

    DrawingPath path;
    Vector3 targetPos;
    bool moving = false;

    private void Start()
    {
        PickRandomTarget();
        StartCoroutine(UpdatePath());
    }

    void PickRandomTarget()
    {
        targetPos = GetRandomWalkablePosition();
        moving = true;
    }

    Vector3 GetRandomWalkablePosition()
    {
        List<WalkingNode> walkableNodes = new List<WalkingNode>();

        for (int x = 0; x < walkingGrid.gridSizeX; x++)
        {
            for (int y = 0; y < walkingGrid.gridSizeY; y++)
            {
                WalkingNode node = walkingGrid.grid[x, y];
                if (node.walkable)
                    walkableNodes.Add(node);
            }
        }

        if (walkableNodes.Count == 0)
            return transform.position;

        WalkingNode randomNode = walkableNodes[Random.Range(0, walkableNodes.Count)];
        return randomNode.worldPosition;
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (moving)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, targetPos, OnPathFound));
            }
            yield return new WaitForSeconds(minPathUpdateTime);
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new DrawingPath(waypoints, transform.position, turnDist, stoppingDist);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        while (true) // keep coroutine alive forever
        {
            if (path == null || !moving)
            {
                yield return null;
                continue;
            }

            int pathIndex = 0;
            transform.LookAt(path.lookPoints[0]);

            while (pathIndex <= path.finishLineIndex)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

                while (pathIndex <= path.finishLineIndex && path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    pathIndex++;
                }

                if (pathIndex > path.finishLineIndex)
                {
                    // Reached the target, pick a new one
                    PickRandomTarget();
                    break; // exit inner loop to request a new path
                }

                Quaternion targetRot = Quaternion.LookRotation(path.lookPoints[Mathf.Min(pathIndex, path.lookPoints.Length - 1)] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * turnDist);

                // Move at full speed
                transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

                yield return null;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}