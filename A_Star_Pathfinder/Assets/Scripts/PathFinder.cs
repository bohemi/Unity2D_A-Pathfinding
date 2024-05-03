using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Transform findMe;
    [SerializeField] Transform finder;

    Grids grid;

    private void Start()
    {
        grid = GetComponent<Grids>();
    }

    void FindPath(Vector2 start, Vector2 end)
    {
        Node startNode = grid.NodeFromWorldPoint(start);
        Node endNode = grid.NodeFromWorldPoint(end);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                ReconstructPath();
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour,endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }

            }
        }
    }

    void ReconstructPath()
    {

    }

    int GetDistance(Node current,Node neighbourNode)
    {
        return 1;
    }
}