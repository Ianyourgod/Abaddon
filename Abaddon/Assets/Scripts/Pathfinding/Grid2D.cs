using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid2D : MonoBehaviour
{
    public Node2D[,] Grid;

    List<Tilemap> obstaclemaps;

    // Cache for physics overlap checks
    private Dictionary<Vector3, bool> obstacleCache = new Dictionary<Vector3, bool>();

    [SerializeField]
    LayerMask collideLayers;

    public List<Node2D> path;
    public Vector3 worldBottomLeft;

    float nodeDiameter;

    [SerializeField]
    public int gridSizeX = 0;

    [SerializeField]
    public int gridSizeY = 0;

    void Start()
    {
        obstaclemaps = new List<Tilemap>();

        if (EnemyMapPropagator.main == null)
            return;

        obstaclemaps.AddRange(EnemyMapPropagator.GetObstacleMaps());
        if (gridSizeX == 0)
        {
            gridSizeX = Mathf.RoundToInt(20);
            gridSizeY = Mathf.RoundToInt(20);
        }
        CreateGrid();
    }

    private bool HasTile(Vector3 worldPosition)
    {
        // Check cache first
        if (obstacleCache.TryGetValue(worldPosition, out bool cachedResult))
            return cachedResult;

        bool hasObstacle = false;

        if (obstaclemaps != null)
        {
            foreach (Tilemap tilemap in obstaclemaps)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
                if (tilemap.HasTile(cellPosition))
                {
                    hasObstacle = true;
                    break; // Early exit if we find a tile
                }
            }
        }

        if (!hasObstacle)
        {
            Vector3 new_position = worldPosition + new Vector3(.5f, .5f, 0);
            if (new_position != transform.position && ObjectIsThere(new_position))
            {
                hasObstacle = true;
            }
        }

        // Cache the result
        obstacleCache[worldPosition] = hasObstacle;
        return hasObstacle;
    }

    private bool ObjectIsThere(Vector3 position)
    {
        float size = 0.9f;
        return Physics2D.OverlapBox(position, new Vector2(size, size), 0, collideLayers) != null;
    }

    public void CreateGrid()
    {
        // Clear cache when recreating grid
        obstacleCache.Clear();

        Grid = new Node2D[gridSizeX, gridSizeY];
        worldBottomLeft =
            transform.position - Vector3.right * gridSizeX / 2 - Vector3.up * gridSizeY / 2;

        int centerX = gridSizeX / 2;
        int centerY = gridSizeY / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * x + Vector3.up * y;
                Grid[x, y] = new Node2D(false, worldPoint, x, y);

                // Use integer comparison instead of Math.Floor for center check
                if (x == centerX && y == centerY)
                {
                    Grid[x, y].SetObstacle(false);
                }
                else
                {
                    Grid[x, y].SetObstacle(HasTile(worldPoint));
                }
            }
        }
    }

    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>(4); // Pre-allocate capacity
        int x = node.GridX;
        int y = node.GridY;

        // Check bounds once and add neighbors directly
        if (y + 1 < gridSizeY) // Top
            neighbors.Add(Grid[x, y + 1]);
        if (y - 1 >= 0) // Bottom
            neighbors.Add(Grid[x, y - 1]);
        if (x + 1 < gridSizeX) // Right
            neighbors.Add(Grid[x + 1, y]);
        if (x - 1 >= 0) // Left
            neighbors.Add(Grid[x - 1, y]);

        return neighbors;
    }

    public Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        // convert it to be relative to the grid
        Vector3 relativePosition = worldPosition - worldBottomLeft;
        int x = Mathf.FloorToInt(relativePosition.x);
        int y = Mathf.FloorToInt(relativePosition.y);

        Node2D node;
        try
        {
            node = Grid[x, y];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogError(
                $"Node at ({x}, {y}) is out of bounds for grid size ({gridSizeX}, {gridSizeY}), returning (0, 0) node."
            );
            node = new Node2D(false, worldPosition, 0, 0);
        }
        return node;
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
                if (n.obstacle)
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(n.worldPosition + halfNodeSize, Vector3.one);
                }
                else
                    Gizmos.color = Color.white;

                if (path != null && path.Contains(n))
                    Gizmos.color = Color.black;
                Gizmos.DrawWireCube(n.worldPosition + halfNodeSize, Vector3.one);
            }
        }
    }
}
