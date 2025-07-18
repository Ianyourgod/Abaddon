using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid2D))]
public class Pathfinding2D : MonoBehaviour
{
    [SerializeField]
    public Grid2D grid;
    Node2D seekerNode,
        targetNode;

    // Pre-allocated collections to avoid garbage collection
    private List<Node2D> openSet = new List<Node2D>();
    private HashSet<Node2D> closedSet = new HashSet<Node2D>();
    private List<Node2D> neighbors = new List<Node2D>();

    public (int, List<Node2D>) FindPath(Vector3 startPos, Vector3 targetPos)
    {
        grid.CreateGrid();

        //get player and target position in grid coords
        seekerNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        // Early exit if start or target is obstacle
        if (seekerNode.obstacle || targetNode.obstacle)
            return (0, null);

        // Clear and reuse collections
        openSet.Clear();
        closedSet.Clear();
        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {
            // Use more efficient node selection with early exit
            Node2D currentNode = GetLowestFCostNode();

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //If target found, retrace path
            if (currentNode == targetNode)
            {
                RetracePath(seekerNode, targetNode);
                return (currentNode.gCost, grid.path);
            }

            //adds neighbor nodes to openSet
            neighbors = grid.GetNeighbors(currentNode);
            for (int i = 0; i < neighbors.Count; i++)
            {
                Node2D neighbour = neighbors[i];
                if (neighbour.obstacle || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return (0, null);
    }

    private Node2D GetLowestFCostNode()
    {
        Node2D node = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (
                openSet[i].FCost < node.FCost
                || (openSet[i].FCost == node.FCost && openSet[i].hCost < node.hCost)
            )
            {
                node = openSet[i];
            }
        }
        return node;
    }

    //reverses calculated path so first node is closest to seeker
    void RetracePath(Node2D startNode, Node2D endNode)
    {
        List<Node2D> path = new List<Node2D>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    //gets distance between 2 nodes for calculating cost
    int GetDistance(Node2D nodeA, Node2D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        // Simplified calculation for 4-directional movement
        return 10 * (dstX + dstY); // Only horizontal/vertical moves allowed
    }
}
