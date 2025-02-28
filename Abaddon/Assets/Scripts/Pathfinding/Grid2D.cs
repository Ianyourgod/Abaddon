using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid2D : MonoBehaviour
{
    public Node2D[,] Grid;
    [SerializeField] Tilemap[] obstaclemaps;
    [SerializeField] LayerMask collideLayers;

    public List<Node2D> path;
    public Vector3 worldBottomLeft;

    float nodeDiameter;
    public int gridSizeX, gridSizeY = 0;

    void Awake()
    {
        if (gridSizeX == 0) {
            gridSizeX = Mathf.RoundToInt(9);
            gridSizeY = Mathf.RoundToInt(9);
        }
        CreateGrid();
    }

    private bool HasTile(Vector3 worldPosition)
    {
        foreach (Tilemap tilemap in obstaclemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            if (tilemap.HasTile(cellPosition))
                return true;
        }
        Vector3 new_position = worldPosition + new Vector3(.5f, .5f, 0);
        if (new_position == transform.position) return false;
        if (ObjectIsThere(new_position)) return true;
        return false;
    }

    private bool ObjectIsThere(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, 0.3f, collideLayers) != null;
    }

    public void CreateGrid()
    {
        Grid = new Node2D[gridSizeX, gridSizeY];
        Vector3 halfNodeSize = Vector3.one * 0.5f;
        worldBottomLeft = transform.position - Vector3.right * gridSizeX / 2 - Vector3.up * gridSizeY / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * x + Vector3.up * y;
                Grid[x, y] = new Node2D(false, worldPoint, x, y);

                if (x == Math.Floor((float) gridSizeX / 2) && y == Math.Floor((float) gridSizeY / 2)) {
                    Grid[x, y].SetObstacle(false);
                } else

                if (HasTile(Grid[x, y].worldPosition))
                    Grid[x, y].SetObstacle(true);
                else
                    Grid[x, y].SetObstacle(false);

            }
        }
    }


    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY + 1]);

        //checks and adds bottom neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY - 1]);

        //checks and adds right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY]);

        //checks and adds left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY]);



        /* Uncomment this code to enable diagonal movement
        
        //checks and adds top right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1< gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

        //checks and adds bottom right neighbor
        if (node.GridX + 1>= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

        //checks and adds top left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1>= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

        //checks and adds bottom left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY  - 1>= 0 && node.GridY  - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
        */



        return neighbors;
    }


    public Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        // convert it to be relative to the grid
        Vector3 relativePosition = worldPosition - worldBottomLeft;
        int x = Mathf.FloorToInt(relativePosition.x);
        int y = Mathf.FloorToInt(relativePosition.y);

        return Grid[x, y];
    }
    
    public void DrawGizmos()
    {
        CreateGrid();
        if (Grid != null)
        {
            Vector3 halfNodeSize = Vector3.one * 0.5f;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, gridSizeY, 1));
            foreach (Node2D n in Grid)
            {
                if (n.obstacle) {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(n.worldPosition + halfNodeSize, Vector3.one);
                } else
                    Gizmos.color = Color.white;

                if (path != null && path.Contains(n))
                    Gizmos.color = Color.black;
                Gizmos.DrawWireCube(n.worldPosition + halfNodeSize, Vector3.one);

            }
        }
    }
}
