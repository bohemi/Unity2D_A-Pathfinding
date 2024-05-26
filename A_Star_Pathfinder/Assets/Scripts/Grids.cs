using System.Collections.Generic;
using UnityEngine;

public class Grids : MonoBehaviour
{
    [SerializeField] LayerMask _unwalkableMask;
    [SerializeField] Vector2 _gridSize;
    [SerializeField] bool _shouldDisplayGround;
    [SerializeField] float _nodeRadius;
    float nodeDiameter;

    Node[,] _grid;
    int _gridSizeX, _gridSizeY;

    void Awake()
    {
        _shouldDisplayGround = false;
        nodeDiameter = _nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(_gridSize.x / nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * _gridSize.x / 2 - Vector2.up * _gridSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + _nodeRadius) + 
                                                       Vector2.up * (y * nodeDiameter + _nodeRadius);
                
                bool walkable = !Physics2D.OverlapCircle(worldPoint, _nodeRadius, _unwalkableMask);
                _grid[x, y] = new Node(walkable, worldPoint, x, y);
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

                int nodeX = node.GridX + x;
                int nodeY = node.GridY + y;

                if (nodeX >= 0 && nodeX < _gridSizeX && nodeY >= 0 && nodeY < _gridSizeY)
                    neighbour.Add(_grid[nodeX, nodeY]);
            }
        }
        return neighbour;
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition) // returns Node of given parameter
    {
        // far left(0), middle(0.5), far right(1)
        float percentX = (worldPosition.x + _gridSize.x / 2) / _gridSize.x;
        float percentY = (worldPosition.y + _gridSize.y / 2) / _gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // -1 so the array would not be out of index. in this case grid is 30 by 30 so array in 0-29
        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _grid[x, y];
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
        // Grid Area visualization
        Gizmos.DrawWireCube(transform.position, new Vector2(_gridSize.x, _gridSize.y));

        if (_grid != null)
        {
            foreach (Node n in _grid)
            {
                if (_shouldDisplayGround)
                {
                    Gizmos.color = n.Walkable ? Color.white : Color.red;
                    // if overlpaCircle detects unwalkable layer then turn that grid to red
                    Gizmos.DrawCube(n.WorldPosition, Vector2.one * (nodeDiameter - .1f));
                }

                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(n.WorldPosition, Vector2.one * (nodeDiameter - .1f));
                    }
                }
            }
        }
    }
}
