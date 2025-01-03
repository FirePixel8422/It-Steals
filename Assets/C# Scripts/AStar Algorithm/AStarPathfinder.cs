using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public static class AStarPathfinder
{
    private static GridManager gridManager;


    public static void Init(GridManager _gridManager)
    {
        gridManager = _gridManager;
    }


    public static bool TryGetPathToTarget(Vector3 startPos, Vector3 targetPos, int visibleTileCost, List<Vector3> path, bool forceAvoidVisibleTiles)
    {
        Node startNode = gridManager.NodeFromWorldPoint(startPos);
        Node targetNode = gridManager.NodeFromWorldPoint(targetPos);

        Heap<Node> openNodes = new Heap<Node>(gridManager.MaxSize);
        HashSet<Node> closedNodes = new HashSet<Node>();


        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.RemoveFirst();
            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                bool pathSucces = RetracePath(startNode, targetNode, path, forceAvoidVisibleTiles);

                return pathSucces;
            }


            foreach (Node neigbour in gridManager.GetNeigbours(currentNode))
            {
                if (!neigbour.walkable || closedNodes.Contains(neigbour))
                {
                    continue;
                }

                int2 currentNodeGridPos = currentNode.gridPos;

                int neigbourDist = GetDistance(currentNodeGridPos, neigbour.gridPos);
                int newMovementCostToNeigbour = currentNode.gCost + neigbourDist;

                if (neigbour.visibleByPlayer)
                {
                    newMovementCostToNeigbour += visibleTileCost;
                }


                if (newMovementCostToNeigbour < neigbour.gCost || !openNodes.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;

                    neigbour.hCost = GetDistance(neigbour.gridPos, targetNode.gridPos);
                    neigbour.parentGridPos = currentNodeGridPos;

                    if (!openNodes.Contains(neigbour))
                    {
                        openNodes.Add(neigbour);
                    }
                }
            }
        }

        return false;
    }

    private static bool RetracePath(Node startNode, Node endNode, List<Vector3> path, bool forceAvoidVisibleTiles)
    {
        path.Clear();

        if (endNode == startNode)
        {
            Debug.LogWarning("Target Already Reached");
            return false;
        }

        Node currentNode = endNode;

        while (currentNode != startNode)
        {

            if (currentNode != endNode && forceAvoidVisibleTiles && currentNode.visibleByPlayer)
            {
                Debug.LogWarning("Scary Mode = " + forceAvoidVisibleTiles + " Failed");

                path.Clear();
                return false;
            }


            path.Add(currentNode.worldPos);

            currentNode = GridManager.NodeFromGridId(currentNode.parentGridPos);
        }

        //reverse when done
        path.Reverse();

        return true;
    }


    private static int GetDistance(int2 gridPosA, int2 gridPosB)
    {
        int distX = math.abs(gridPosA.x - gridPosB.x);
        int distZ = math.abs(gridPosA.y - gridPosB.y);

        if (distX > distZ)
        {
            return 14 * distZ + 10 * (distX - distZ);
        }
        else
        {
            return 14 * distX + 10 * (distZ - distX);
        }
    }
}
