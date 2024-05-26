using UnityEngine;

public class Node
{
    public bool Walkable;
    public Vector2 WorldPosition;
    public int GridX;
    public int GridY;

    public int GCost;
    public int HCost;
    // if we find the best node from our neighbours then we set the parent of that node to this
    public Node Parent;

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        Walkable = walkable;
        WorldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
    }

    public int fCost
    {
        get
        {
            return GCost + HCost;
        }
    }
}
