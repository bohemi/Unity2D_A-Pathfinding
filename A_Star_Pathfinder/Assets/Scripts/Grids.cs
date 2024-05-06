using System.Collections.Generic;
using UnityEngine;

public class Grids : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableMask;
    [SerializeField] Vector2 gridSize;
    [SerializeField] float nodeRadius;
    float nodeDiameter;

    Node[,] grid;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridSize.x / 2 - Vector2.up * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + 
                                                       Vector2.up * (y * nodeDiameter + nodeRadius);
                
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbour = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int nodeX = node.gridX + x;
                int nodeY = node.gridY + y;

                if (nodeX >= 0 && nodeX < gridSizeX && nodeY >= 0 && nodeY < gridSizeY)
                    neighbour.Add(grid[nodeX, nodeY]);
            }
        }
        return neighbour;
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition) // returns gridPosition at the given parameter
    {
        // far left(0), middle(0.5), far right(1)
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // -1 so the array would not be out of index. in this case grid is 30 by 30 so array in 0-29
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridSize.x, gridSize.y)); // Grid Area

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // if overlpaCircle detects unwalkable layer then turn that grid to red
                Gizmos.color = n.walkable ? Color.green : Color.red;

                if (path != null)
                {
                    if (path.Contains(n))
                    { Gizmos.color = Color.yellow; }
                }

                Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - .1f));
            }
        }
    }
}
