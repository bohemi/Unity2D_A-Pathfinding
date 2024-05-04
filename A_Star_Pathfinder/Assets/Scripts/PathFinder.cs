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

        openSet.Add(startNode); // At the beginning open will only contain the starting node

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // ** the first time entry inside the below loop will be skipped. Because at the start
            // openset contains 1 node and forLoop also starts from 1. since at the beginning we dont
            // even have any values for comparison and no neighbours.

            // since currentNode will always be the first of the openset, we will start comparing openSet
            // ( starting from openset[1] ) to currentNode that we retrieve from neighbourNodes
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                {
                    if (openSet[i].hCost < currentNode.hCost)
                    { currentNode = openSet[i]; }
                }
            }

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

                // in the first iter. of all neighbour,all the gCost will be zero since we are setting values here.
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
